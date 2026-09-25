namespace HotelBooking.Contracts.Bookings.Responses;

public sealed record CheckoutResponse(int BookingId, string ConfirmationNumber, decimal TotalPrice, bool IsNew);