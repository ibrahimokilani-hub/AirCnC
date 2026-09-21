using FluentValidation;

namespace HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(command => command.Name).ValidCityName();
        RuleFor(command => command.Country).ValidCountry();
        RuleFor(command => command.PostOffice).ValidPostOffice();
    }
}