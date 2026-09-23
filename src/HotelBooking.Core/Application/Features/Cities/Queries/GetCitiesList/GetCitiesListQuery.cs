using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesList;

public sealed record GetCitiesListQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    string? SortDirection)
    : IQuery<PagedResult<CityListItem>>
{
    public static readonly string[] SortableColumns = ["name", "country", "createdAt"];
}