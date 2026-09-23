using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;

namespace HotelBooking.Core.Application.Features.Users.Queries;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize).InclusiveBetween(1, ListRules.MaxPageSize);

    }
}