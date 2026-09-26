using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;

namespace HotelBooking.Core.Application.Features.Reviews.Queries;

public sealed class GetHotelReviewsQueryValidator : AbstractValidator<GetHotelReviewsQuery>
{
    public GetHotelReviewsQueryValidator()
    {
        RuleFor(reviews => reviews.HotelId).GreaterThanOrEqualTo(0);
        
        RuleFor(reviews => reviews.Page).ValidPage();
        RuleFor(reviews => reviews.PageSize).ValidPageSize();
    }
}
