using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCityById;

public sealed class GetCityByIdQueryHandler(IAppDbContext context) : IQueryHandler<GetCityByIdQuery, CityResponse>
{
    public async Task<Result<CityResponse>> HandleAsync(
        GetCityByIdQuery query,
        CancellationToken cancellationToken)
    {
        var city = await context.Cities
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(city => new CityResponse(
                city.Id,
                city.Name,
                city.Country,
                city.PostOffice
            ))
            .FirstOrDefaultAsync(cancellationToken);
        if (city is null)
        {
            return Result<CityResponse>.Failure(CityErrors.NotFound(query.Id));
        }
        
        return Result<CityResponse>.Success(city);

    }

}