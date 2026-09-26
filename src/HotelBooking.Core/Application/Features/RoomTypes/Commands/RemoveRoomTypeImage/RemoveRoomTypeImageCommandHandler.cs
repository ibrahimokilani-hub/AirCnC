using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.RemoveRoomTypeImage;

public sealed class RemoveRoomTypeImageCommandHandler(IAppDbContext context, IHotelOwnership ownership)
    : ICommandHandler<RemoveRoomTypeImageCommand>
{
    public async Task<Result> HandleAsync(RemoveRoomTypeImageCommand command, CancellationToken cancellationToken)
    {
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }

        var roomTypeInHotel = await context.RoomTypes.AnyAsync(
            roomType => roomType.Id == command.RoomTypeId && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (!roomTypeInHotel)
        {
            return Result.Failure(Error.NotFound(
                "RoomType.NotFound",
                $"Hotel '{command.HotelId}' has no room type with ID '{command.RoomTypeId}'."));
        }

        var image = await context.RoomTypeImages.FirstOrDefaultAsync(
            image => image.Id == command.ImageId && image.RoomTypeId == command.RoomTypeId,
            cancellationToken);

        if (image is null)
        {
            return Result.Failure(Error.NotFound(
                "RoomTypeImage.NotFound",
                $"Room type '{command.RoomTypeId}' has no image with ID '{command.ImageId}'."));
        }

        context.RoomTypeImages.Remove(image);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
