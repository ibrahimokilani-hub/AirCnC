using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.UpdateRoomType;

public sealed class UpdateRoomTypeCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<UpdateRoomTypeCommand> validator)
    : ICommandHandler<UpdateRoomTypeCommand>
{
    public async Task<Result> HandleAsync(UpdateRoomTypeCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToValidationError());
        }

        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }

        var roomType = await context.RoomTypes.FirstOrDefaultAsync(
            roomType => roomType.Id == command.Id && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (roomType is null)
        {
            return Result.Failure(RoomTypeErrors.NotFound(command.HotelId, command.Id));
        }

        roomType.Update(command.Name, command.Description, command.PricePerNight, command.MaxAdults, command.MaxChildren);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}