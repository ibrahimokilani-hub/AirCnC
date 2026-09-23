using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Rooms;

public static class RoomErrors
{
    public static Error NotFound(int hotelId, int id) =>
        Error.NotFound("Room.NotFound", $"Hotel '{hotelId}' has no room with ID '{id}'.");

    public static Error NumberTaken(string number) =>
        Error.Conflict("Room.NumberTaken", $"This hotel already has a room '{number}'.");

    public static ValidationError RoomTypeNotInHotel(int roomTypeId) =>
        new(new Dictionary<string, string[]>
        {
            ["RoomTypeId"] = [$"Room type '{roomTypeId}' does not belong to this hotel."]
        });
}