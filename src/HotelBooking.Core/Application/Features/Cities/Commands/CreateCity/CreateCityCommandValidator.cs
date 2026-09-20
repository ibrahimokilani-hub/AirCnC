using FluentValidation;

namespace HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(70);

        RuleFor(command => command.Country)
            .NotEmpty().WithMessage("Country name is required.")
            .MaximumLength(40);

        RuleFor(command => command.PostOffice)
            .NotEmpty().WithMessage("Post office is required.")
            .MaximumLength(15);
    }
}
