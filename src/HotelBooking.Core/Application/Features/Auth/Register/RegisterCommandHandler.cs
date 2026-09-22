using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(IIdentityService identityService, IValidator<RegisterCommand> validator) : ICommandHandler<RegisterCommand, int>
{
    public async Task<Result<int>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);

        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var newUser = new NewUser(
            command.Email.Trim(),
            command.Password,
            command.FirstName.Trim(),
            command.LastName.Trim()
        );
        
        return await identityService.RegisterAsync(newUser, cancellationToken);
    }
}