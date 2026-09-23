namespace HotelBooking.Contracts.Rooms;

public sealed record RoomListItem(
    int Id,
    string Number,
    int RoomTypeId,
    string RoomTypeName,
    int MaxAdults,
    int MaxChildren,
    bool IsAvailableToday,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);