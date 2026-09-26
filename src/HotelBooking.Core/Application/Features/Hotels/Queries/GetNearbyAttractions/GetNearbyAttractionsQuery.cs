using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetNearbyAttractions;

public sealed record GetNearbyAttractionsQuery(int Page, int PageSize, string? Search, string? SortBy, string? SortDirection)
    : IQuery<PagedResult<NearbyAttractionResponse>>;
