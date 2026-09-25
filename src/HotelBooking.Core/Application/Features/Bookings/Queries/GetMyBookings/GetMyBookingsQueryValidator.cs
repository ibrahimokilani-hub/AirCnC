using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Features.Bookings.Queries.GetMyBookings;

public sealed class GetMyBookingsQueryValidator : AbstractValidator<GetMyBookingsQuery>
{
    public GetMyBookingsQueryValidator()
    {
        RuleFor(query => query.Page).ValidPage();
        RuleFor(query => query.PageSize).ValidPageSize();
    }
}