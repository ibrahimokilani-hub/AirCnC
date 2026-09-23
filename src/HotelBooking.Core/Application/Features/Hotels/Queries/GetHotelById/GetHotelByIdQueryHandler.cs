using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelById;

public sealed class GetHotelByIdQueryHandler(
    IAppDbContext context,
    ICurrentUser currentUser)
    : IQueryHandler<GetHotelByIdQuery, HotelResponse>
{
    public async Task<Result<HotelResponse>> HandleAsync(GetHotelByIdQuery query, CancellationToken cancellationToken)
    {
        
        var hotel = await context.Hotels
            .AsNoTracking()
            .Where(hotel => hotel.Id == query.Id && hotel.OwnerId == currentUser.UserId)
            .Select(hotel => new HotelResponse(
                hotel.Id,
                hotel.CityId,
                hotel.City.Name, // a JOIN, because the projection reads it; no Include needed
                hotel.Name,
                hotel.OwnerId,
                hotel.Owner.FirstName + " " + hotel.Owner.LastName,
                hotel.Description,
                hotel.StarRating,
                hotel.HotelType.ToString(),
                hotel.Address,
                hotel.Latitude,
                hotel.Longitude,
                hotel.Amenities.Select(am => am.Id).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return hotel is null
            ? Result<HotelResponse>.Failure(HotelErrors.NotFound(query.Id))
            : Result<HotelResponse>.Success(hotel);
    }
}