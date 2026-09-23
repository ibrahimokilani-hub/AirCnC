using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelsList;

public sealed class GetHotelsListQueryValidator : AbstractValidator<GetHotelsListQuery>
{
    public GetHotelsListQueryValidator()
    {
        RuleFor(query => query.Page).ValidPage();
        RuleFor(query => query.PageSize).ValidPageSize();
        RuleFor(query => query.Search).MaximumLength(100);
        RuleFor(query => query.SortBy).OneOf(GetHotelsListQuery.SortableColumns);
        RuleFor(query => query.SortDirection).ValidSortDirection();
    }
}