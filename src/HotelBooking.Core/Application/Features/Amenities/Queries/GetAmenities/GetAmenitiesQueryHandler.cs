using HotelBooking.Contracts.Amenities;
using HotelBooking.Contracts.Amenities.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Amenities.Queries.GetAmenities;

public sealed class GetAmenitiesQueryHandler(IAppDbContext context)
    : IQueryHandler<GetAmenitiesQuery, IReadOnlyList<AmenityResponse>>
{
    public async Task<Result<IReadOnlyList<AmenityResponse>>> HandleAsync(
        GetAmenitiesQuery query,
        CancellationToken cancellationToken)
    {
        var amenities = await context.Amenities
            .AsNoTracking()
            .OrderBy(amenity => amenity.Name)
            .Select(amenity => new AmenityResponse(amenity.Id, amenity.Name))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AmenityResponse>>.Success(amenities);
    }
}