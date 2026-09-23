using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Amenities.Commands.CreateAmenity;

public sealed class CreateAmenityCommandValidator : AbstractValidator<CreateAmenityCommand>
{
    public CreateAmenityCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Amenity.NameMaxLength);
    }
}