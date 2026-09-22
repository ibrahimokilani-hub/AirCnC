using FluentValidation;
using HotelBooking.Contracts.Auth.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IValidator<LoginCommand> validator)
    : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<AuthResponse>.Failure(validation.ToValidationError());
        }

        var email = command.Email.Trim().ToLowerInvariant();

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidCreds);
        }
        
        var verifiedPass = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verifiedPass)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidCreds);
        }

        var token = tokenService.CreateAccessToken(new AuthUser(user.Id, user.Email, user.FirstName, user.LastName, user.Role));
        
        return Result<AuthResponse>.Success(new AuthResponse(token.Token, token.ExpiresAtUtc));
    }
}