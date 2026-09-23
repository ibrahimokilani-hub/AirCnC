using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelsList;

public sealed record GetHotelsListQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    string? SortDirection)
    : IQuery<PagedResult<HotelListItem>>
{
    public static readonly string[] SortableColumns = ["name", "city", "starRating", "createdAt"];
}