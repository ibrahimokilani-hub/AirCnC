using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddHotelImage;

public sealed class AddHotelImageCommandValidator : AbstractValidator<AddHotelImageCommand>
{
    public AddHotelImageCommandValidator()
    {
        RuleFor(command => command.ImageUrl).NotEmpty().MaximumLength(HotelImage.ImageUrlMaxLength);
    }
}
