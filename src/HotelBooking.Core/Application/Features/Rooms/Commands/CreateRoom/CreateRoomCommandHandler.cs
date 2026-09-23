using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Hotels;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<CreateRoomCommand> validator)
    : ICommandHandler<CreateRoomCommand, int>
{
    public async Task<Result<int>> HandleAsync(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return Result<int>.Failure(access.Error!);
        }

        var typeInHotel = await context.RoomTypes.AnyAsync(
            roomType => roomType.Id == command.RoomTypeId && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (!typeInHotel)
        {
            return Result<int>.Failure(RoomErrors.RoomTypeNotInHotel(command.RoomTypeId));
        }

        var number = command.Number.Trim();

        var numberTaken = await context.Rooms.AnyAsync(
            room => room.HotelId == command.HotelId && room.Number == number,
            cancellationToken);

        if (numberTaken)
        {
            return Result<int>.Failure(RoomErrors.NumberTaken(number));
        }

        var room = Room.Create(command.HotelId, command.RoomTypeId, number);

        context.Rooms.Add(room);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(room.Id);
    }
}
