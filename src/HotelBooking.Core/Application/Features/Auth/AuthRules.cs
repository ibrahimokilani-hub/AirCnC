using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Auth;

public static class AuthRules
{
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Email is required.")
            .EmailAddress();

    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> rule) 
        => rule.NotEmpty().WithMessage("{PropertyName} is required.") 
            .MaximumLength(User.NameMaxLength);
    
    public static IRuleBuilderOptions<T, string> ValidPhone<T>(this IRuleBuilder<T, string> rule) => rule
        .NotEmpty().WithMessage("Phone number is required.")
        .MaximumLength(User.PhoneMaxLength)
        .Matches(@"^\+?[0-9 ]{7,20}$")
        .WithMessage("Enter a phone number with digits only, like +970 599 123 456.");

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) => rule
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6);
}