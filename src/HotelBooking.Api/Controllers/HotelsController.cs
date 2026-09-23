using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Requests;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelDetails;
using HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

/// <summary>
/// The guest-facing hotel page. A different class from Admin.HotelsController: different
/// audience, different data, different authorization. They share nothing but a noun.
/// </summary>
[AllowAnonymous]
[Route("api/v1/hotels")]
[Tags("Hotels")]
public sealed class HotelsController(
    IQueryHandler<GetHotelDetailsQuery, HotelDetailsResponse> getHotelDetails,
    IQueryHandler<GetRoomAvailabilityQuery, IReadOnlyList<RoomTypeAvailabilityResponse>> getRoomAvailability)
    : ApiController
{
    /// <summary>Everything the hotel page shows: details, amenities, and map pins</summary>
    /// <response code="200">The hotel.</response>
    /// <response code="404">No such hotel (or it was deleted).</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType<ApiResponse<HotelDetailsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails(int id, CancellationToken cancellationToken)
    {
        var result = await getHotelDetails.HandleAsync(new GetHotelDetailsQuery(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }

    /// <summary>Every room type with its price and how many rooms are free for the dates</summary>
    /// <response code="200">{ "data": [ ... ] }, cheapest first.</response>
    /// <response code="400">Invalid dates or guests.</response>
    /// <response code="404">No such hotel.</response>
    [HttpGet("{id:int:min(1)}/rooms")]
    [ProducesResponseType<ApiResponse<IReadOnlyList<RoomTypeAvailabilityResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomAvailability(
        int id,
        [FromQuery] RoomAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetRoomAvailabilityQuery(id, request.CheckIn, request.CheckOut, request.Adults, request.Children);

        var result = await getRoomAvailability.HandleAsync(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }
}
