using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetPopularCities;

public sealed class GetPopularCitiesQueryHandler(IAppDbContext context)
    : IQueryHandler<GetPopularCitiesQuery, IReadOnlyList<CityBookingCountResponse>>
{
    private const int TopCount = 5;

    public async Task<Result<IReadOnlyList<CityBookingCountResponse>>> HandleAsync(
        GetPopularCitiesQuery query,
        CancellationToken cancellationToken)
    {
        var topCityCounts = await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.Status == BookingStatus.Confirmed)
            .Join(
                context.Hotels,
                booking => booking.HotelId,
                hotel => hotel.Id,
                (booking, hotel) => hotel.CityId)
            .GroupBy(cityId => cityId)
            .Select(group => new
            {
                CityId = group.Key,
                BookingCount = group.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(TopCount)
            .ToListAsync(cancellationToken);

        var cityIds = topCityCounts.Select(x => x.CityId).ToList();

        var cities = await context.Cities
            .AsNoTracking()
            .Where(city => cityIds.Contains(city.Id))
            .Select(city => new CityResponse(
                city.Id,
                city.Name,
                city.Country,
                city.PostOffice,
                city.CityImageUrl))
            .ToDictionaryAsync(city => city.Id, cancellationToken);

        var result = topCityCounts
            .Select(x => new CityBookingCountResponse(cities[x.CityId], x.BookingCount))
            .ToList();

        return Result<IReadOnlyList<CityBookingCountResponse>>.Success(result);
    }
}