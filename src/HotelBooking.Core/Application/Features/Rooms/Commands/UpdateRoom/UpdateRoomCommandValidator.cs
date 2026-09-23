using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.UpdateRoom;

public sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(command => command.Number).NotEmpty().MaximumLength(Room.NumberMaxLength);
        RuleFor(command => command.RoomTypeId).GreaterThan(0);
    }
}