using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.AddRoomTypeImage;

public sealed class AddRoomTypeImageCommandValidator : AbstractValidator<AddRoomTypeImageCommand>
{
    public AddRoomTypeImageCommandValidator()
    {
        RuleFor(command => command.ImageUrl).NotEmpty().MaximumLength(RoomTypeImage.ImageUrlMaxLength);
    }
}
