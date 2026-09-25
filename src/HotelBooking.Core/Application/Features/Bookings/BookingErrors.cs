using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Bookings;

public static class BookingErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Booking.NotFound", $"You have no booking with ID '{id}'.");

    public static ValidationError UnknownRoomType(int roomTypeId) =>
        new(new Dictionary<string, string[]>
        {
            ["RoomTypeId"] = [$"Room type '{roomTypeId}' doesn't exist."]
        });

    public static ValidationError TooManyGuests(string roomTypeName) =>
        new(new Dictionary<string, string[]>
        {
            ["Adults"] = [$"That many guests don't fit in one '{roomTypeName}'."]
        });

    // The guest details are copied from the account (D-20), so an account without a phone
    // can't produce a valid booking. Rows created before Users.Phone existed have "".
    public static ValidationError ProfileIncomplete =>
        new(new Dictionary<string, string[]>
        {
            ["Phone"] = ["Add a phone number to your profile before booking."]
        });

    public static Error RoomTypeSoldOut(string roomTypeName, DateOnly checkIn, DateOnly checkOut) =>
        Error.Conflict(
            "Booking.SoldOut",
            $"'{roomTypeName}' is no longer available from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd}. Pick other dates or another room type.");
}
