using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Home;
using HotelBooking.Contracts.Home.Responses;
using HotelBooking.Contracts.Reviews.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Home.Discounts.Queries.GetFeaturedDeals;
using HotelBooking.Core.Application.Features.Reviews.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

/// <summary>The home page's sections (brief §2).</summary>
[Route("api/v1/home")]
[Tags("Home")]
public sealed class HomeController(
    IQueryHandler<GetFeaturedDealsQuery, IReadOnlyList<FeaturedDealResponse>> getFeaturedDeals, IQueryHandler<GetHotelReviewsQuery, PagedResult<ReviewResponse>> getReviews)
    : ApiController
{
    /// <summary>Up to 5 hotels with a discount today, biggest first. Cached for 5 minutes.</summary>
    [AllowAnonymous]
    [HttpGet("featured-deals")]
    [ProducesResponseType<ApiResponse<IReadOnlyList<FeaturedDealResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeaturedDeals(CancellationToken cancellationToken)
    {
        var result = await getFeaturedDeals.HandleAsync(new GetFeaturedDealsQuery(), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }
    
    /// <summary>The hotel's guest reviews, newest first.</summary>
    /// <response code="200">{ "data": [ ... ], "meta": { ... } }.</response>
    /// <response code="404">No such hotel.</response>
    [AllowAnonymous]
    [HttpGet("{id:int:min(1)}/reviews")]
    [ProducesResponseType<PagedResponse<ReviewResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviews(
        int id,
        [FromQuery] FilteredRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await getReviews.HandleAsync(new GetHotelReviewsQuery(id, request.Page, request.PageSize), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }
}