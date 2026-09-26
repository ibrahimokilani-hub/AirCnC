using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Reviews.Commands.WriteReview;

public sealed record WriteReviewCommand(int BookingId, int Rating, string Comment) : ICommand<int>;
