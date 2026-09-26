using FluentValidation;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Reviews.Commands.WriteReview;

public sealed class WriteReviewCommandValidator : AbstractValidator<WriteReviewCommand>
{
    public WriteReviewCommandValidator()
    {
        RuleFor(command => command.Rating).InclusiveBetween(Review.MinRating, Review.MaxRating);

        RuleFor(command => command.Comment).NotEmpty().MaximumLength(Review.CommentMaxLength);
    }
}
