using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(IRefreshTokenService refreshTokens) : ICommandHandler<LogoutCommand>
{
    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            await refreshTokens.RevokeAsync(command.RefreshToken, cancellationToken);
        }

        return Result.Success();
    }
}