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
    /// <response code="200">This checkout already went through; "data" is that same booking.</response>
    /// <response code="400">Unknown room type, guests who don't fit, dates in the past or out of order, or a missing Idempotency-Key header.</response>
    /// <response code="409">A room type is no longer available for those dates.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType<ApiResponse<CheckoutResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResponse<CheckoutResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Checkout(
        [FromBody] CheckoutRequest request,
        [FromHeader(Name = ApiHeaders.IdempotencyKey)] Guid? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new CheckoutCommand(
            request.RoomTypeId,
            request.CheckIn,
            request.CheckOut,
            request.Adults,
            request.Children,
            request.Notes,
            idempotencyKey ?? Guid.Empty);

        var result = await checkout.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        // 201 means THIS request created it. A retry did not, so it gets 200 with the same
        // booking: a client counting 201s can tell "Booked!" from "You already booked this".
        return result.Value.IsNew
            ? StatusCode(StatusCodes.Status201Created, new ApiResponse<CheckoutResponse>(result.Value))
            : Ok(new ApiResponse<CheckoutResponse>(result.Value));
    }
}