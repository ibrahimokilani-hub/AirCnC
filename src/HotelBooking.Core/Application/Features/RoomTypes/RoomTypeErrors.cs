using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.RoomTypes;

public static class RoomTypeErrors
{
    public static Error NotFound(int hotelId, int roomTypeId) =>
        Error.NotFound(
            "RoomType.NotFound",
            $"Hotel '{hotelId}' has no room type with ID '{roomTypeId}'.");
    
    public static Error HasRooms(int id) =>
        Error.Conflict("RoomType.HasRooms", $"Rooms still use room type '{id}'. Move or delete them first.");
}