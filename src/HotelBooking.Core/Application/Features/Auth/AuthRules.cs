using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Auth;

public static class AuthRules
{
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Emmail is required.")
            .EmailAddress();

    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> rule) 
        => rule.NotEmpty().WithMessage("{PropertyName} is required.") 
            .MaximumLength(City.NameMaxLength);
    
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) => rule
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6);
}