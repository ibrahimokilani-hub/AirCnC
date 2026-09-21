using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Cities;

public static class CityRules
{
    public static IRuleBuilderOptions<T, string> ValidCityName<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("City name is required.")
            .MaximumLength(City.NameMaxLength);

    public static IRuleBuilderOptions<T, string> ValidCountry<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Country name is required.")
            .MaximumLength(City.CountryMaxLength);

    public static IRuleBuilderOptions<T, string> ValidPostOffice<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Post office is required.")
            .MaximumLength(City.PostOfficeMaxLength);
}