using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.RemoveNearbyAttraction;

public sealed class RemoveNearbyAttractionCommandHandler(IAppDbContext context)
    : ICommandHandler<RemoveNearbyAttractionCommand>
{
    public async Task<Result> HandleAsync(RemoveNearbyAttractionCommand command, CancellationToken cancellationToken)
    {
        var attraction = await context.NearbyAttractions.FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);
        if (attraction is null)
        {
            return Result.Failure(Error.NotFound(
                "NearbyAttraction.NotFound",
                $"The attraction with ID '{command.Id}' was not found."));
        }

        context.NearbyAttractions.Remove(attraction);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
