using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Reviews;

public static class ReviewErrors
{
    public static Error AlreadyReviewed(int bookingId) =>
        Error.Conflict("Review.AlreadyReviewed", $"You have already reviewed booking '{bookingId}'.");
}