using FluentValidation;
using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Availability;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;

public class CheckoutCommandHandler(IAppDbContext context, ICurrentUser currentUser, TimeProvider timeProvider, IValidator<CheckoutCommand> validator) : ICommandHandler<CheckoutCommand, CheckoutResponse>
{
    public async Task<Result<CheckoutResponse>> HandleAsync(CheckoutCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<CheckoutResponse>.Failure(validation.ToValidationError());
        }

        if (currentUser.UserId is not { } userId)
        {
            return Result<CheckoutResponse>.Failure(AuthErrors.NotAuthenticated);
        }


        // Did this checkout attempt already go through? One seek on the unique index.
        var existing = await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.UserId == userId && booking.IdempotencyKey == command.IdempotencyKey)
            .Select(booking => new CheckoutResponse(
                booking.Id,
                Booking.ConfirmationPrefix + booking.Id.ToString(),
                booking.TotalPrice,
                false))
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
        {
            return Result<CheckoutResponse>.Success(existing);
        }

        var roomType = await context.RoomTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(type => type.Id == command.RoomTypeId, cancellationToken);

        if (roomType is null)
        {
            return Result<CheckoutResponse>.Failure(BookingErrors.UnknownRoomType(command.RoomTypeId));
        }

        if (!roomType.Fits(command.Adults, command.Children))
        {
            return Result<CheckoutResponse>.Failure(BookingErrors.TooManyGuests(roomType.Name));
        }

        var stay = DateRange.Create(command.CheckIn, command.CheckOut);

        // Pick one concrete free room of that type, lowest number first.
        var roomId = await context.Rooms
            .Where(room => room.RoomTypeId == command.RoomTypeId)
            .Where(RoomAvailability.IsFreeDuring(stay))
            .OrderBy(room => room.Number)
            .Select(room => (int?)room.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (roomId is null)
        {
            return Result<CheckoutResponse>.Failure(
                BookingErrors.RoomTypeSoldOut(roomType.Name, command.CheckIn, command.CheckOut));
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);

        var booking = Booking.Create(
            userId,
            roomType.HotelId,
            command.IdempotencyKey,
            command.Notes,
            now);

        var hotel = await context.Hotels.AsNoTracking().Where(hotel => hotel.Id == roomType.HotelId).FirstOrDefaultAsync(cancellationToken);

        var discount = await context.Hotels
            .AsNoTracking()
            .Where(h => h.Id == booking.HotelId)
            .SelectMany(h => h.Discounts)
            .Where(discount =>
                discount.StartsAt <= today &&
                discount.EndsAt >= today)
            .FirstOrDefaultAsync(cancellationToken);

        var originalPrice = roomType.PricePerNight;

        var finalPrice = discount is null
            ? originalPrice
            : discount.DiscountType switch
            {
                DiscountType.Percentage =>
                    originalPrice * (100m - discount.Value) / 100m,

                DiscountType.FixedAmount =>
                    Math.Max(originalPrice - discount.Value, 0m),

                _ => throw new ArgumentOutOfRangeException()
            };

        // The price is copied into the item: tomorrow's price change never moves it.
        booking.AddItem(roomId.Value, stay, command.Adults, command.Children, finalPrice);

        context.Bookings.Add(booking);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CheckoutResponse>.Success(
            new CheckoutResponse(booking.Id, booking.ConfirmationNumber, booking.TotalPrice, true));
    }
}
