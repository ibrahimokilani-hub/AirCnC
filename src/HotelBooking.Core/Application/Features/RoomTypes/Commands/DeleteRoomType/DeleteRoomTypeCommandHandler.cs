using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.DeleteRoomType;

public sealed class DeleteRoomTypeCommandHandler(IAppDbContext context) : ICommandHandler<DeleteRoomTypeCommand>
{
    public async Task<Result> HandleAsync(DeleteRoomTypeCommand command, CancellationToken cancellationToken)
    {
        var roomType = await context.RoomTypes.FirstOrDefaultAsync(
            roomType => roomType.Id == command.Id && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (roomType is null)
        {
            return Result.Failure(RoomTypeErrors.NotFound(command.HotelId, command.Id));
        }
        
        if (await context.Rooms.AnyAsync(room => room.RoomTypeId == command.Id, cancellationToken))
        {
            return Result.Failure(RoomTypeErrors.HasRooms(command.Id));
        }

        context.RoomTypes.Remove(roomType);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}