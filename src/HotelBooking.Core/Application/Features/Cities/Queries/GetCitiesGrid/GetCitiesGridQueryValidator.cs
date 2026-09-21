using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesGrid;

public sealed class GetCitiesGridQueryValidator : AbstractValidator<GetCitiesGridQuery>
{
    public GetCitiesGridQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize).InclusiveBetween(1, GridRules.MaxPageSize);

        RuleFor(query => query.Search).MaximumLength(GridRules.MaxSearchLength);

        // Absent (null) is fine: the handler sorts by name. Anything else must be in the list.
        RuleFor(query => query.SortBy)
            .Must(sortBy => GetCitiesGridQuery.SortableColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .When(query => query.SortBy is not null)
            .WithMessage($"Must be one of: {string.Join(", ", GetCitiesGridQuery.SortableColumns)}.");

        RuleFor(query => query.SortDirection)
            .Must(direction => GridRules.SortDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
            .When(query => query.SortDirection is not null)
            .WithMessage("Must be asc or desc.");
    }
}