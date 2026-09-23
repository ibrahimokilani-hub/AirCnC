using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Search;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Search.SearchHotels.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[AllowAnonymous]
[Route("api/v1/search")]
[Tags("Search")]
public sealed class SearchController(
    IQueryHandler<SearchHotelsQuery, PagedResult<SearchHotelItem>> searchHotels)
    : ApiController
{
    /// <summary>Hotels with enough free rooms for the guests and dates, filtered and sorted.</summary>
    /// <remarks>
    /// Defaults: check-in today, check-out the day after, 2 adults, 0 children, 1 room.
    /// sortBy: price (default), stars, name. Anonymous: guests search before logging in.
    /// </remarks>
    /// <response code="200">One page of results.</response>
    /// <response code="400">Dates in the past, more rooms than adults, an unknown filter value.</response>
    [HttpGet("hotels")]
    [ProducesResponseType<PagedResponse<SearchHotelItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchHotels([FromQuery] SearchHotelsRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchHotelsQuery(
            request.Query,
            request.CheckIn,
            request.CheckOut,
            request.Adults,
            request.Children,
            request.Rooms,
            request.MinPrice,
            request.MaxPrice,
            request.MinStars,
            request.AmenityIds,
            request.HotelType,
            request.SortBy,
            request.SortDirection,
            request.Page,
            request.PageSize);

        var result = await searchHotels.HandleAsync(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }
}