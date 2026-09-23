using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.DeleteRoom;

public sealed class DeleteRoomCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider,
    IHotelOwnership ownership)
    : ICommandHandler<DeleteRoomCommand>
{
    public async Task<Result> HandleAsync(DeleteRoomCommand command, CancellationToken cancellationToken)
    {
        // Ownership of the parent hotel first: a room is only as protected as its hotel
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
        
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var hasUpcoming = await context.BookingItems.AnyAsync(
            item => item.RoomId == command.Id && item.IsActive && item.CheckOut > today,
            cancellationToken);

        if (hasUpcoming)
        {
            return Result.Failure(RoomErrors.HasUpcomingBookings(command.Id));
        }

        context.Rooms.Remove(room);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}