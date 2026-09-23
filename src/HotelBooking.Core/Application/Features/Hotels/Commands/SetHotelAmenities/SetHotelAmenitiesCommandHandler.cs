using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Amenities;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.SetHotelAmenities;

public sealed class SetHotelAmenitiesCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<SetHotelAmenitiesCommand> validator)
    : ICommandHandler<SetHotelAmenitiesCommand>
{
    public async Task<Result> HandleAsync(SetHotelAmenitiesCommand command, CancellationToken cancellationToken)
    {
        var hotel = await context.Hotels
            .Include(hotel => hotel.Amenities)
            .FirstOrDefaultAsync(hotel => hotel.Id == command.HotelId, cancellationToken);
        
        var access = await ownership.EnsureOwnerAsync(hotel!.OwnerId, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToValidationError());
        }
        
        if (hotel is null)
        {
            return Result.Failure(HotelErrors.NotFound(command.HotelId));
        }

        var requestedIds = command.AmenityIds.Distinct().ToList();

        var amenities = await context.Amenities
            .Where(amenity => requestedIds.Contains(amenity.Id))
            .ToListAsync(cancellationToken);

        var unknownIds = requestedIds.Except(amenities.Select(amenity => amenity.Id)).ToList();
        
        if (unknownIds.Count > 0)
        {
            return Result.Failure(AmenityErrors.Unknown(unknownIds));
        }

        hotel.SetAmenities(amenities);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
