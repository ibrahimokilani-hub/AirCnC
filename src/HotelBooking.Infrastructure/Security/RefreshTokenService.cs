using System.Security.Cryptography;
using System.Text;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Infrastructure.Options;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HotelBooking.Infrastructure.Security;

public class RefreshTokenService(
    AppDbContext context,
    IOptions<JwtOptions> options,
    TimeProvider timeProvider)
    : IRefreshTokenService
{
    private readonly TimeSpan _lifetime = TimeSpan.FromDays(options.Value.RefreshTokenDays);

    public async Task<IssuedRefreshToken> IssueAsync(int userId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now + _lifetime;
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        context.RefreshTokens.Add(RefreshToken.Create(userId, Hash(token), now, expires));
        await context.SaveChangesAsync(cancellationToken);

        return new IssuedRefreshToken(token, expires);
    }

    public async Task<Result<int>> ValidateAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var stored = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(token => token.TokenHash == Hash(refreshToken), cancellationToken);

        return stored is not null && stored.IsActive(now)
            ? Result<int>.Success(stored.UserId)
            : Result<int>.Failure(AuthErrors.InvalidRefresh);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        await context.RefreshTokens
            .Where(token => token.TokenHash == Hash(refreshToken) && token.RevokedAtUtc == null)
            .ExecuteUpdateAsync(set => set.SetProperty(token => token.RevokedAtUtc, now), cancellationToken);
    }

    public static string Hash(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
