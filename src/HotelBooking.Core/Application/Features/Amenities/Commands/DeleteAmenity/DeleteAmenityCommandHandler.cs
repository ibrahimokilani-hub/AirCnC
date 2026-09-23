using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Amenities.Commands.DeleteAmenity;


public sealed class DeleteAmenityCommandHandler(IAppDbContext context) : ICommandHandler<DeleteAmenityCommand>
{
    public async Task<Result> HandleAsync(DeleteAmenityCommand command, CancellationToken cancellationToken)
    {
        var amenity = await context.Amenities.FirstOrDefaultAsync(amenity => amenity.Id == command.Id, cancellationToken);
        if (amenity is null)
        {
            return Result.Failure(AmenityErrors.NotFound(command.Id));
        }

        context.Amenities.Remove(amenity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}