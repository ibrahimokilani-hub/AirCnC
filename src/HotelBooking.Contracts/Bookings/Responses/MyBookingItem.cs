namespace HotelBooking.Contracts.Bookings.Responses;

public sealed record MyBookingItem(
    int Id,
    string ConfirmationNumber,
    string HotelName,
    string CityName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Rooms,
    string Status,
    decimal TotalPrice,
    DateTime CreatedAtUtc);