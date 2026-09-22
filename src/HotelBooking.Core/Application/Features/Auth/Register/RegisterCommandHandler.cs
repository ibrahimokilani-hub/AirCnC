using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Auth.Register;

/// <summary>
/// A user is a row like any other: checked, created and saved through IAppDbContext,
/// the same way a city is. The only special step is hashing the password first.
/// </summary>
public sealed class RegisterCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider,
    IValidator<RegisterCommand> validator)
    : ICommandHandler<RegisterCommand, int>
{
    public async Task<Result<int>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var email = command.Email.Trim().ToLowerInvariant();

        if (await context.Users.AnyAsync(user => user.Email == email, cancellationToken))
        {
            return Result<int>.Failure(AuthErrors.EmailTaken(email));
        }

        var user = User.Create(
            email,
            command.FirstName,
            command.LastName,
            passwordHasher.Hash(command.Password),
            timeProvider.GetUtcNow().UtcDateTime);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(user.Id);
    }
}
