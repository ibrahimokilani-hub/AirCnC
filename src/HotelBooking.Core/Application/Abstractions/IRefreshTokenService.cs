using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Abstractions;

public interface IRefreshTokenService
{
    // Login: create and store a new refresh token
    Task<IssuedRefreshToken> IssueAsync(int userId, CancellationToken cancellationToken);

    // Refresh: the token must exist, not be expired, and not be revoked. Returns the owner's id.
    Task<Result<int>> ValidateAsync(string refreshToken, CancellationToken cancellationToken);

    // Logout: revoke the token
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}

public sealed record IssuedRefreshToken(string Token, DateTime ExpiresAtUtc);
