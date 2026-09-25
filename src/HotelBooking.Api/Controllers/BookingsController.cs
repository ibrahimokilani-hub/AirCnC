using HotelBooking.Contracts.Bookings;
using HotelBooking.Contracts.Bookings.Requests;
using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

/// <summary>The logged-in guest's bookings. Login required by the fallback policy.</summary>
[Route("api/v1/bookings")]
[Tags("Bookings")]
public sealed class BookingsController(
    ICommandHandler<CheckoutCommand, CheckoutResponse> checkout)
    : ApiController
{
    /// <summary>Books one room for one stay. The guest's details come from the logged-in account.</summary>
    /// <response code="201">Booked. "data" holds the booking id and confirmation number.</response>
    /// <response code="400">Unknown room type, guests who don't fit, dates in the past or out of order, or an account with no phone number.</response>
    /// <response code="409">A room type is no longer available for those dates.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType<ApiResponse<CheckoutResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
    {
        var command = new CheckoutCommand(
            request.RoomTypeId,
            request.CheckIn,
            request.CheckOut,
            request.Adults,
            request.Children,
            request.Notes);

        var result = await checkout.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<CheckoutResponse>(result.Value));
    }
}