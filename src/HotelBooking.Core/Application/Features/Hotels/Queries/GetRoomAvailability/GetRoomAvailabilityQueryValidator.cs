using FluentValidation;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;

public sealed class GetRoomAvailabilityQueryValidator : AbstractValidator<GetRoomAvailabilityQuery>
{
    public GetRoomAvailabilityQueryValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(query => query.CheckIn)
            .GreaterThanOrEqualTo(today).WithMessage("Check-in can't be in the past.")
            .When(query => query.CheckIn is not null);

        RuleFor(query => query.CheckOut)
            .GreaterThan(query => query.CheckIn ?? today).WithMessage("Check-out must be after check-in.")
            .LessThanOrEqualTo(query => (query.CheckIn ?? today).AddDays(DateRange.MaxNights))
            .WithMessage($"A stay can be at most {DateRange.MaxNights} nights.")
            .When(query => query.CheckOut is not null);

        RuleFor(query => query.Adults).InclusiveBetween(1, 20);
        RuleFor(query => query.Children).InclusiveBetween(0, 20);
    }
}