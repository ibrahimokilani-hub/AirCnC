using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelDetails;

public sealed class GetHotelDetailsQueryHandler(IAppDbContext context)
    : IQueryHandler<GetHotelDetailsQuery, HotelDetailsResponse>
{
    public async Task<Result<HotelDetailsResponse>> HandleAsync(
        GetHotelDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var hotel = await context.Hotels
            .AsNoTracking()
            .Where(hotel => hotel.Id == query.Id)
            .Select(hotel => new HotelDetailsResponse(
                hotel.Id,
                hotel.Name,
                hotel.City.Name,
                hotel.City.Country,
                hotel.StarRating,
                hotel.HotelType.ToString(),
                hotel.Description,
                hotel.Address,
                hotel.Latitude,
                hotel.Longitude,
                hotel.Amenities
                    .OrderBy(amenity => amenity.Name)
                    .Select(amenity => amenity.Name)
                    .ToList(),
                hotel.NearbyAttractions
                    .OrderBy(attraction => attraction.DistanceMeters)
                    .Select(attraction => new NearbyAttractionResponse(
                        attraction.Id,
                        attraction.Name,
                        attraction.Category,
                        attraction.Latitude,
                        attraction.Longitude,
                        attraction.DistanceMeters))
                    .ToList()))
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        return hotel is null
            ? Result<HotelDetailsResponse>.Failure(HotelErrors.NotFound(query.Id))
            : Result<HotelDetailsResponse>.Success(hotel);
    }
}
