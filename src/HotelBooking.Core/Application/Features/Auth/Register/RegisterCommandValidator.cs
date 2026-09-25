using FluentValidation;

namespace HotelBooking.Core.Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email).ValidEmail();
        RuleFor(command => command.FirstName).ValidName();
        RuleFor(command => command.LastName).ValidName();
        RuleFor(command => command.Phone).ValidPhone();
        RuleFor(command => command.Password).ValidPassword();
    }
}