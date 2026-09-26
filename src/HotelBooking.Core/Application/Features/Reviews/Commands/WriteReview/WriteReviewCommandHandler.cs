using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Application.Features.Bookings;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Reviews.Commands.WriteReview;

public sealed class WriteReviewCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IValidator<WriteReviewCommand> validator)
    : ICommandHandler<WriteReviewCommand, int>
{
    public async Task<Result<int>> HandleAsync(WriteReviewCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        if (currentUser.UserId is not { } userId)
        {
            return Result<int>.Failure(AuthErrors.NotAuthenticated);
        }

        var booking = await context.Bookings
            .Include(booking => booking.Items)
            .FirstOrDefaultAsync(booking => booking.Id == command.BookingId && booking.UserId == userId, cancellationToken);

        if (booking is null)
        {
            return Result<int>.Failure(BookingErrors.NotFound(command.BookingId));
        }

        // same user trying to write a review twice... LIKE HELL you are!!
        if (await context.Reviews.AnyAsync(review => review.BookingId == booking.Id, cancellationToken))
        {
            return Result<int>.Failure(ReviewErrors.AlreadyReviewed(booking.Id));
        }

        // Snapshot the author's name onto the review, so it reads the same later even if
        // the account is renamed. The booking knows UserId, not the display name.
        var reviewer = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new { user.FirstName, user.LastName })
            .FirstOrDefaultAsync(cancellationToken);

        if (reviewer is null)
        {
            return Result<int>.Failure(AuthErrors.NotAuthenticated);
        }

        var reviewerName = $"{reviewer.FirstName} {reviewer.LastName}";

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var written = booking.WriteReview(reviewerName, command.Rating, command.Comment, DateOnly.FromDateTime(now), now);
        if (written.IsFailure)
        {
            return Result<int>.Failure(written.Error!);
        }

        context.Reviews.Add(written.Value);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(written.Value.Id);
    }
}
