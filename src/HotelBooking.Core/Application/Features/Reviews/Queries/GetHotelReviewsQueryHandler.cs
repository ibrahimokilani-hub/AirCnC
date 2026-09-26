using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Reviews.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Hotels;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Reviews.Queries;

public sealed class GetHotelReviewsQueryHandler(IAppDbContext context, IValidator<GetHotelReviewsQuery> validator)
    : IQueryHandler<GetHotelReviewsQuery, PagedResult<ReviewResponse>>
{
    public async Task<Result<PagedResult<ReviewResponse>>> HandleAsync(
        GetHotelReviewsQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<ReviewResponse>>.Failure(validation.ToValidationError());
        }

        if (!await context.Hotels.AnyAsync(hotel => hotel.Id == query.HotelId, cancellationToken))
        {
            return Result<PagedResult<ReviewResponse>>.Failure(HotelErrors.NotFound(query.HotelId));
        }

        // The reviewer name is stored on the review (a snapshot), so this is a plain
        // projection — no join to Users, nothing per-row.
        var page = await context.Reviews
            .AsNoTracking()
            .Where(review => review.HotelId == query.HotelId)
            .OrderByDescending(review => review.CreatedAtUtc)
            .ThenByDescending(review => review.Id)
            .Select(review => new ReviewResponse(
                review.Id,
                review.Rating,
                review.ReviewerName,
                review.Comment,
                review.CreatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<ReviewResponse>>.Success(page);
    }
}
