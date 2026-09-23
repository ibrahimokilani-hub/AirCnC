namespace HotelBooking.Contracts.RoomTypes.Responses;

public sealed record RoomTypeResponse(
    int Id,
    string Name,
    string Description,
    decimal PricePerNight,
    int MaxAdults,
    int MaxChildren);