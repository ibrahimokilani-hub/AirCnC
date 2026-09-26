using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.RemoveHotelImage;

public sealed class RemoveHotelImageCommandHandler(IAppDbContext context, IHotelOwnership ownership)
    : ICommandHandler<RemoveHotelImageCommand>
{
    public async Task<Result> HandleAsync(RemoveHotelImageCommand command, CancellationToken cancellationToken)
    {
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }

        var image = await context.HotelImages.FirstOrDefaultAsync(
            image => image.Id == command.ImageId && image.HotelId == command.HotelId,
            cancellationToken);

        if (image is null)
        {
            return Result.Failure(Error.NotFound(
                "HotelImage.NotFound",
                $"Hotel '{command.HotelId}' has no image with ID '{command.ImageId}'."));
        }

        context.HotelImages.Remove(image);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
