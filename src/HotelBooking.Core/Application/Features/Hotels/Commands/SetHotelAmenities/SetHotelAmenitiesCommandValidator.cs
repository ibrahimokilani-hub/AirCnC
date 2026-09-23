using FluentValidation;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.SetHotelAmenities;

public sealed class SetHotelAmenitiesCommandValidator : AbstractValidator<SetHotelAmenitiesCommand>
{
    public SetHotelAmenitiesCommandValidator()
    {
        RuleFor(command => command.AmenityIds).NotNull();
        RuleForEach(command => command.AmenityIds).GreaterThan(0);
    }
}