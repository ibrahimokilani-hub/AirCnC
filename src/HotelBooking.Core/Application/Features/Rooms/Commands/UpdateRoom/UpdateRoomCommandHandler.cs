using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.UpdateRoom;

public sealed class UpdateRoomCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<UpdateRoomCommand> validator)
    : ICommandHandler<UpdateRoomCommand>
{
    public async Task<Result> HandleAsync(UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToValidationError());
        }

        // Ownership of the parent hotel first: a room is only as protected as its hotel.
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }

        var room = await context.Rooms.FirstOrDefaultAsync(
            room => room.Id == command.Id && room.HotelId == command.HotelId,
            cancellationToken);

        if (room is null)
        {
            return Result.Failure(RoomErrors.NotFound(command.HotelId, command.Id));
        }

        var typeInHotel = await context.RoomTypes.AnyAsync(
            roomType => roomType.Id == command.RoomTypeId && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (!typeInHotel)
        {
            return Result.Failure(RoomErrors.RoomTypeNotInHotel(command.RoomTypeId));
        }

        var number = command.Number.Trim();

        var numberTaken = await context.Rooms.AnyAsync(
            other => other.HotelId == command.HotelId && other.Id != command.Id && other.Number == number,
            cancellationToken);

        if (numberTaken)
        {
            return Result.Failure(RoomErrors.NumberTaken(number));
        }

        room.Update(command.RoomTypeId, number);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
