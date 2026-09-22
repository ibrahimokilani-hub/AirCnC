namespace HotelBooking.Contracts.Auth.Responses;

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc);