using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.RemoveNearbyAttraction;

public sealed class RemoveNearbyAttractionCommandHandler(IAppDbContext context, IHotelOwnership ownership)
    : ICommandHandler<RemoveNearbyAttractionCommand>
{
    public async Task<Result> HandleAsync(RemoveNearbyAttractionCommand command, CancellationToken cancellationToken)
    {
        
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }
        
        var attraction = await context.NearbyAttractions.FirstOrDefaultAsync(
            attraction => attraction.Id == command.Id && attraction.HotelId == command.HotelId,
            cancellationToken);

        if (attraction is null)
        {
            return Result.Failure(Error.NotFound(
                "NearbyAttraction.NotFound",
                $"Hotel '{command.HotelId}' has no attraction with ID '{command.Id}'."));
        }

        context.NearbyAttractions.Remove(attraction);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}