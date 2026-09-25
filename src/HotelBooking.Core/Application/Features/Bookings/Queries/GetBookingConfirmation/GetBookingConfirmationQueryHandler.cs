using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Bookings.Queries.GetBookingConfirmation;

public sealed class GetBookingConfirmationQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    : IQueryHandler<GetBookingConfirmationQuery, BookingConfirmationResponse>
{
    public async Task<Result<BookingConfirmationResponse>> HandleAsync(
        GetBookingConfirmationQuery query,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result<BookingConfirmationResponse>.Failure(AuthErrors.NotAuthenticated);
        }
        
        var confirmation = await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.Id == query.BookingId && booking.UserId == userId)
            .Select(booking => new
            {
                Booking = booking,
                Hotel = context.Hotels
                    .IgnoreQueryFilters() 
                    .Where(hotel => hotel.Id == booking.HotelId)
                    .Select(hotel => new { hotel.Name, hotel.Address, CityName = hotel.City.Name, hotel.City.Country })
                    .First(),
                Guest = context.Users
                    .Where(user => user.Id == booking.UserId)
                    .Select(user => new { user.FirstName, user.LastName, user.Email, user.Phone })
                    .First(),
                Rooms = booking.Items
                    .OrderBy(item => item.CheckIn)
                    .Select(item => new BookedRoomResponse(
                        context.Rooms.IgnoreQueryFilters().Where(room => room.Id == item.RoomId).Select(room => room.Number).First(),
                        context.Rooms.IgnoreQueryFilters().Where(room => room.Id == item.RoomId).Select(room => room.RoomType.Name).First(),
                        item.CheckIn,
                        item.CheckOut,
                        item.Nights,
                        item.Adults,
                        item.Children,
                        item.PricePerNight,
                        item.PricePerNight * item.Nights))
                    .ToList()
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (confirmation is null)
        {
            return Result<BookingConfirmationResponse>.Failure(BookingErrors.NotFound(query.BookingId));
        }

        return Result<BookingConfirmationResponse>.Success(new BookingConfirmationResponse(
            confirmation.Booking.Id,
            confirmation.Booking.ConfirmationNumber,
            confirmation.Booking.Status.ToString(),
            confirmation.Booking.CreatedAtUtc,
            confirmation.Hotel.Name,
            confirmation.Hotel.Address,
            confirmation.Hotel.CityName,
            confirmation.Hotel.Country,
            $"{confirmation.Guest.FirstName} {confirmation.Guest.LastName}",
            confirmation.Guest.Email,
            confirmation.Guest.Phone,
            confirmation.Booking.Notes,
            confirmation.Rooms,
            confirmation.Booking.TotalPrice));
    }
}
