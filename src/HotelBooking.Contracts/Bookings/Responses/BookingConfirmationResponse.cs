namespace HotelBooking.Contracts.Bookings.Responses;

public sealed record BookingConfirmationResponse(
    int Id,
    string ConfirmationNumber,
    string Status,
    DateTime CreatedAtUtc,
    string HotelName,
    string HotelAddress,
    string CityName,
    string Country,
    string GuestName,
    string GuestEmail,
    string GuestPhone,
    string? Notes,
    IReadOnlyList<BookedRoomResponse> Rooms,
    decimal TotalPrice);

public sealed record BookedRoomResponse(
    string RoomNumber,
    string RoomTypeName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Nights,
    int Adults,
    int Children,
    decimal PricePerNight,
    decimal Subtotal);