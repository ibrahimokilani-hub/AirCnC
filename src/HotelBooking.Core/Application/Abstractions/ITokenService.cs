using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(AuthUser user);
}

public sealed record AuthUser(int Id, string Email, string FirstName, string LastName, UserRole Role);

public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
