namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record RoomTypeAvailabilityResponse(
    int RoomTypeId,
    string Name,
    string Description,
    decimal PricePerNight,
    int MaxAdults,
    int MaxChildren,
    int AvailableRooms,
    bool FitsGuests);