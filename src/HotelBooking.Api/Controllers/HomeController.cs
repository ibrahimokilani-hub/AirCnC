using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Home;
using HotelBooking.Contracts.Home.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Home.Discounts.Queries.GetFeaturedDeals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

/// <summary>The home page's sections (brief §2).</summary>
[Route("api/v1/home")]
[Tags("Home")]
public sealed class HomeController(
    IQueryHandler<GetFeaturedDealsQuery, IReadOnlyList<FeaturedDealResponse>> getFeaturedDeals)
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
}