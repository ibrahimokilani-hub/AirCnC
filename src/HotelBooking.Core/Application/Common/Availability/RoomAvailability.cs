using System.Linq.Expressions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Application.Common.Availability;

public static class RoomAvailability
{
    public static Expression<Func<Room, bool>> IsFreeDuring(DateRange stay)
    {
        var checkIn = stay.CheckIn;
        var checkOut = stay.CheckOut;

        return room => !room.BookingItems.Any(item =>
            item.IsActive &&
            item.CheckIn < checkOut &&
            checkIn < item.CheckOut);
    }
}