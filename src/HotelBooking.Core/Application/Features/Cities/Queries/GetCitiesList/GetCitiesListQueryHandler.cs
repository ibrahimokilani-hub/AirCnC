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

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesList;

public sealed record GetCitiesListQueryHandler(
    IAppDbContext context,
    IValidator<GetCitiesListQuery> validator
    ): IQueryHandler<GetCitiesListQuery, PagedResult<CityListItem>>
{
    public async Task<Result<PagedResult<CityListItem>>> HandleAsync(
        GetCitiesListQuery query,
        CancellationToken cancellationToken
    )
    {
        var validatation = await validator.ValidateAsync(query, cancellationToken);

        if (!validatation.IsValid)
        {
            return Result<PagedResult<CityListItem>>.Failure(validatation.ToValidationError());
        }

        IQueryable<City> cities = context.Cities.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            cities = cities.Where(city => city.Name.Contains(search) || city.Country.Contains(search) || city.PostOffice.Contains(search));
        }
        
        var page = await Sort(cities, query.SortBy, query.SortDirection)
            .Select(city => new CityListItem(
                city.Id,
                city.Name,
                city.Country,
                city.PostOffice,
                city.Hotels.Count,
                city.CreatedAtUtc,
                city.UpdatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<CityListItem>>.Success(page);
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