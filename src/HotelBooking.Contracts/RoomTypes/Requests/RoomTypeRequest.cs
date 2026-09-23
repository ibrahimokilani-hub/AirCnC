namespace HotelBooking.Contracts.RoomTypes.requests;

public sealed record RoomTypeRequest(
    string Name,
    string Description,
    decimal PricePerNight,
    int MaxAdults,
    int MaxChildren);