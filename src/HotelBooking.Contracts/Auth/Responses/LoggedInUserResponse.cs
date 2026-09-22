namespace HotelBooking.Contracts.Auth.Responses;

public sealed record LoggedInUserResponse(int Id, string Email, string FirstName, string LastName, string Role);