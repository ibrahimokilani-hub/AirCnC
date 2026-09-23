using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Search;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Search.SearchHotels.Queries;

public sealed record SearchHotelsQuery(
    string? Query,
    DateOnly? CheckIn,
    DateOnly? CheckOut,
    int Adults,
    int Children,
    int Rooms,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinStars,
    IReadOnlyList<int> AmenityIds,
    string? HotelType,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize)
    : IQuery<PagedResult<SearchHotelItem>>
{
    public static readonly string[] SortableColumns = ["price", "stars", "name"];
}