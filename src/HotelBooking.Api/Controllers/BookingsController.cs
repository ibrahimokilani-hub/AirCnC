using HotelBooking.Contracts.Bookings;
using HotelBooking.Contracts.Bookings.Requests;
using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Reviews.Requests;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;
using HotelBooking.Core.Application.Features.Bookings.Queries.GetBookingConfirmation;
using HotelBooking.Core.Application.Features.Bookings.Queries.GetMyBookings;
using HotelBooking.Core.Application.Features.Reviews.Commands.WriteReview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

/// <summary>The logged-in guest's bookings. Login required by the fallback policy.</summary>
[Route("api/v1/bookings")]
[Tags("Bookings")]
public sealed class BookingsController(
    ICommandHandler<CheckoutCommand, CheckoutResponse> checkout,
    ICommandHandler<WriteReviewCommand, int> writeReview,
    IQueryHandler<GetBookingConfirmationQuery, BookingConfirmationResponse> getConfirmation,
    IQueryHandler<GetMyBookingsQuery, PagedResult<MyBookingItem>> getMyBookings)
    : ApiController
{

    [Authorize]
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings([FromQuery] FilteredRequest request, CancellationToken cancellationToken)
    {
        var result = await getMyBookings.HandleAsync(new GetMyBookingsQuery(request.Page, request.PageSize), cancellationToken);
        
        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkPaged(result.Value);
    }
    
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

        return result.Value.IsNew
            ? StatusCode(StatusCodes.Status201Created, new ApiResponse<CheckoutResponse>(result.Value))
            : Ok(new ApiResponse<CheckoutResponse>(result.Value));
    }
    
    /// <summary>Reviews the hotel of one of my finished stays. One review per booking.</summary>
    /// <remarks>
    /// Under the booking, not POST /hotels/{id}/reviews: the booking is what proves I
    /// stayed there, and it already knows the hotel.
    /// </remarks>
    /// <response code="201">Written. "data" holds the review id.</response>
    /// <response code="400">Rating not 1-5, or an empty or too long comment.</response>
    /// <response code="404">No such booking, or it isn't mine.</response>
    /// <response code="409">Already reviewed, cancelled, or the stay isn't over yet.</response>
    [HttpPost("{id:int:min(1)}/review")]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> WriteReview(int id, [FromBody] ReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await writeReview.HandleAsync(new WriteReviewCommand(id, request.Rating, request.Comment), cancellationToken);

        // 201 without a Location header: there's no "GET one review" endpoint to point at.
        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    [HttpGet("{id:int:min(1)}/confirmation")]
    public async Task<IActionResult> GetConfirmationById(int BookingId, CancellationToken cancellationToken)
    {
        var result = await getConfirmation.HandleAsync(new GetBookingConfirmationQuery(BookingId), cancellationToken);
        
        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkData(result.Value);
    }
}