using FluentValidation;
namespace HotelBooking.Core.Application.Features.Cities.Commands.UpdateCity;

public sealed class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        RuleFor(command => command.Name).ValidCityName();
        RuleFor(command => command.Country).ValidCountry();
        RuleFor(command => command.PostOffice).ValidPostOffice();
    }
}