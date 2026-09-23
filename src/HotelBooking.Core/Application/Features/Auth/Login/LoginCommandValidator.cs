using FluentValidation;

namespace HotelBooking.Core.Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .ValidEmail();
        RuleFor(command => command.Password)
            .ValidPassword();
    }
}