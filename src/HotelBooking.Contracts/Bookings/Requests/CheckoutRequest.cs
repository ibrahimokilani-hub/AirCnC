namespace HotelBooking.Contracts.Bookings.Requests;

public sealed record CheckoutRequest(
    int RoomTypeId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Adults,
    int Children,
    string? Notes);
