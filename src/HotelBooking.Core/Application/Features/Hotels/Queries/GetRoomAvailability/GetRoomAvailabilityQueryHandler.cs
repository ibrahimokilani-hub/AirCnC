using FluentValidation;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Availability;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;

public sealed class GetRoomAvailabilityQueryHandler(
    IAppDbContext context,
    IValidator<GetRoomAvailabilityQuery> validator,
    TimeProvider timeProvider)
    : IQueryHandler<GetRoomAvailabilityQuery, IReadOnlyList<RoomTypeAvailabilityResponse>>
{
    public async Task<Result<IReadOnlyList<RoomTypeAvailabilityResponse>>> HandleAsync(
    GetRoomAvailabilityQuery query,
    CancellationToken cancellationToken)
{
    
    // validation
    var validation = await validator.ValidateAsync(query, cancellationToken);
    if (!validation.IsValid)
    {
        return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>
            .Failure(validation.ToValidationError());
    }

    if (!await context.Hotels.AnyAsync(
        hotel => hotel.Id == query.HotelId,
        cancellationToken))
    {
        return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>
            .Failure(HotelErrors.NotFound(query.HotelId));
    }

    // collect relative data
    var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
    var checkIn = query.CheckIn ?? today;
    var stay = DateRange.Create(
        checkIn,
        query.CheckOut ?? checkIn.AddDays(1));

    var freeRooms = context.Rooms
        .Where(RoomAvailability.IsFreeDuring(stay));

    // look if there are any active discounts for the current hotel
    var activeDiscount = await context.Discounts
        .AsNoTracking()
        .Where(discount =>
            discount.IsActive &&
            discount.HotelId == query.HotelId &&
            discount.StartsAt <= today &&
            discount.EndsAt >= today)
        .FirstOrDefaultAsync(cancellationToken);

    // get the room types for the current hotel
    var roomTypes = await context.RoomTypes
        .AsNoTracking()
        .Where(roomType => roomType.HotelId == query.HotelId)
        .OrderBy(roomType => roomType.PricePerNight)
        .ThenBy(roomType => roomType.Id)
        .Select(roomType => new
        {
            roomType.Id,
            roomType.Name,
            roomType.Description,
            roomType.PricePerNight,
            roomType.MaxAdults,
            roomType.MaxChildren,
            AvailableRooms = freeRooms.Count(
                room => room.RoomTypeId == roomType.Id),
            IsSuitable =
                roomType.MaxAdults >= query.Adults &&
                roomType.MaxChildren >= query.Children
        })
        .ToListAsync(cancellationToken);

    // generate the response (all room types available) with the correct price, after discount applies.
    var response = roomTypes
        .Select(roomType =>
        {
            // correct the price after available discounts
            var discountedPrice = activeDiscount is null
                ? roomType.PricePerNight
                : activeDiscount.DiscountType switch
                {
                    DiscountType.Percentage =>
                        roomType.PricePerNight *
                        (100m - activeDiscount.Value) / 100m,

                    DiscountType.FixedAmount =>
                        Math.Max(
                            roomType.PricePerNight - activeDiscount.Value,
                            0m),

                    _ => throw new ArgumentOutOfRangeException(
                        nameof(activeDiscount.DiscountType))
                };
            
            // return each room with the correct price
            return new RoomTypeAvailabilityResponse(
                roomType.Id,
                roomType.Name,
                roomType.Description,
                decimal.Round(discountedPrice, 2),
                roomType.MaxAdults,
                roomType.MaxChildren,
                roomType.AvailableRooms,
                roomType.IsSuitable);
        })
        .ToList();

    return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>
        .Success(response);
}
}
