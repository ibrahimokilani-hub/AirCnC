using HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;
using HotelBooking.Core.Domain.ValueObjects;
using HotelBooking.Tests.Unit.TestDoubles;

namespace HotelBooking.Tests.Unit.Hotels;

public sealed class GetRoomAvailabilityQueryValidatorTests
{
    private readonly GetRoomAvailabilityQueryValidator _validator = new(FixedClock.AtToday());

    private static GetRoomAvailabilityQuery Valid() =>
        new(1, FixedClock.Today, FixedClock.Today.AddDays(2), 2, 0);

    private string[] FailedProperties(GetRoomAvailabilityQuery query) =>
        _validator.Validate(query).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    /// <summary>The hotel page opens with no dates picked yet.</summary>
    [Fact]
    public void Validate_WithoutDates_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid() with { CheckIn = null, CheckOut = null }).IsValid);

    [Fact]
    public void Validate_CheckInInThePast_HasCheckInError() =>
        Assert.Contains(
            nameof(GetRoomAvailabilityQuery.CheckIn),
            FailedProperties(Valid() with { CheckIn = FixedClock.Today.AddDays(-1) }));

    [Fact]
    public void Validate_CheckOutNotAfterCheckIn_HasCheckOutError() =>
        Assert.Contains(
            nameof(GetRoomAvailabilityQuery.CheckOut),
            FailedProperties(Valid() with { CheckOut = FixedClock.Today }));

    [Fact]
    public void Validate_StayLongerThanTheMaximum_HasCheckOutError()
    {
        var query = Valid() with { CheckOut = FixedClock.Today.AddDays(DateRange.MaxNights + 1) };

        Assert.Contains(nameof(GetRoomAvailabilityQuery.CheckOut), FailedProperties(query));
    }

    [Fact]
    public void Validate_StayExactlyAtTheMaximum_HasNoError()
    {
        var query = Valid() with { CheckOut = FixedClock.Today.AddDays(DateRange.MaxNights) };

        Assert.DoesNotContain(nameof(GetRoomAvailabilityQuery.CheckOut), FailedProperties(query));
    }
}