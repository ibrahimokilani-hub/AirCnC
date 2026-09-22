namespace HotelBooking.Contracts.Users.Responses;

public sealed record UserListItem(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    DateTime CreatedAtUtc);