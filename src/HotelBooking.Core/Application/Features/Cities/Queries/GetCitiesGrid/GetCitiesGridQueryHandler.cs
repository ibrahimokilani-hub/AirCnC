using FluentValidation;
using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesGrid;

public sealed record GetCitiesGridQueryHandler(
    IAppDbContext context,
    IValidator<GetCitiesGridQuery> validator
    ): IQueryHandler<GetCitiesGridQuery, PagedResult<CityGridItem>>
{
    public async Task<Result<PagedResult<CityGridItem>>> HandleAsync(
        GetCitiesGridQuery query,
        CancellationToken cancellationToken
    )
    {
        var validatation = await validator.ValidateAsync(query, cancellationToken);

        if (!validatation.IsValid)
        {
            return Result<PagedResult<CityGridItem>>.Failure(validatation.ToValidationError());
        }

        IQueryable<City> cities = context.Cities.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            cities = cities.Where(city => city.Name.Contains(search) || city.Country.Contains(search) || city.PostOffice.Contains(search));
        }
        
        var page = await Sort(cities, query.SortBy, query.SortDirection)
            .Select(city => new CityGridItem(
                city.Id,
                city.Name,
                city.Country,
                city.PostOffice,
                city.CreatedAtUtc,
                city.UpdatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<CityGridItem>>.Success(page);
    }
    private static IOrderedQueryable<City> Sort(IQueryable<City> cities, string? sortBy, string? direction)
    {
        var descending = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        var sorted = sortBy?.ToLowerInvariant() switch
        {
            null or "name" => descending ? cities.OrderByDescending(c => c.Name) : cities.OrderBy(c => c.Name),
            "country" => descending ? cities.OrderByDescending(c => c.Country) : cities.OrderBy(c => c.Country),
            "createdat" => descending ? cities.OrderByDescending(c => c.CreatedAtUtc) : cities.OrderBy(c => c.CreatedAtUtc),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, "The validator should have rejected this.")
        };

        return sorted.ThenBy(c => c.Id);
    }
}