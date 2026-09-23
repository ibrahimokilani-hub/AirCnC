using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesList;

public sealed class GetCitiesListQueryValidator : AbstractValidator<GetCitiesListQuery>
{
    public GetCitiesListQueryValidator()
    {
        RuleFor(query => query.Page).ValidPage();

        RuleFor(query => query.PageSize).ValidPageSize();

        RuleFor(query => query.Search).ValidSearch();

        RuleFor(query => query.SortBy).OneOf(GetCitiesListQuery.SortableColumns);
        RuleFor(query => query.SortDirection).ValidSortDirection();
    }
}