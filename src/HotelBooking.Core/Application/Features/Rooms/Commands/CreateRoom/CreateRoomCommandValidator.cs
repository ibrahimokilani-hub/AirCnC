using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(command => command.Number).NotEmpty().MaximumLength(Room.NumberMaxLength);
        RuleFor(command => command.RoomTypeId).GreaterThan(0);
    }
}