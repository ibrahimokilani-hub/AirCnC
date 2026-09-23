using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Auth.Refresh;

public sealed class RefreshCommandHandler(
    IRefreshTokenService refreshTokens,
    IAppDbContext context,
    ITokenService tokenService)
    : ICommandHandler<RefreshCommand, AccessToken>
{
    public async Task<Result<AccessToken>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result<AccessToken>.Failure(AuthErrors.InvalidRefresh);
        }

        var validation = await refreshTokens.ValidateAsync(command.RefreshToken, cancellationToken);
        if (validation.IsFailure)
        {
            return Result<AccessToken>.Failure(validation.Error!);
        }

        var user = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == validation.Value)
            .Select(user => new AuthUser(user.Id, user.Email, user.FirstName, user.LastName, user.Role))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result<AccessToken>.Failure(AuthErrors.InvalidRefresh)
            : Result<AccessToken>.Success(tokenService.CreateAccessToken(user));
    }
}
