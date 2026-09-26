using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetNearbyAttractions;

public sealed class GetNearbyAttractionsQueryValidator : AbstractValidator<GetNearbyAttractionsQuery>
{
    public GetNearbyAttractionsQueryValidator()
    {
        RuleFor(query => query.Page).ValidPage();
        RuleFor(query => query.PageSize).ValidPageSize();
    }
}
