using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddNearbyAttraction;

public sealed class AddNearbyAttractionCommandValidator : AbstractValidator<AddNearbyAttractionCommand>
{
    public AddNearbyAttractionCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(NearbyAttraction.NameMaxLength);
        RuleFor(command => command.Category).NotEmpty().MaximumLength(NearbyAttraction.CategoryMaxLength);
        RuleFor(command => command.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(command => command.Longitude).InclusiveBetween(-180m, 180m);
    }
}