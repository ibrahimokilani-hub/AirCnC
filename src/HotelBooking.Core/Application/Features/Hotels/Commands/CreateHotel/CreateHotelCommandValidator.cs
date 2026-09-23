using FluentValidation;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.CreateHotel;

public abstract class CreateHotelValidator<T> : AbstractValidator<CreateHotelCommand>
{
    protected CreateHotelValidator()
    {
        RuleFor(hotel => hotel.CityId).GreaterThan(0);
        RuleFor(hotel => hotel.Name).NotEmpty().MaximumLength(Hotel.NameMaxLength);
        RuleFor(hotel => hotel.Description).NotNull().MaximumLength(Hotel.DescriptionMaxLength);
        RuleFor(hotel => hotel.StarRating).InclusiveBetween(Hotel.MinStars, Hotel.MaxStars);
        RuleFor(hotel => hotel.HotelType)
            .NotEmpty()
            .IsEnumName(typeof(HotelType), caseSensitive: false)
            .WithMessage($"Must be one of: {string.Join(", ", Enum.GetNames<HotelType>())}.");
        RuleFor(hotel => hotel.Address).NotEmpty().MaximumLength(Hotel.AddressMaxLength);
        RuleFor(hotel => hotel.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(hotel => hotel.Longitude).InclusiveBetween(-180m, 180m);
    }
}