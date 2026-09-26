using HotelBooking.Core.Application.Features.Search.SearchHotels.Queries;
using HotelBooking.Core.Domain.ValueObjects;
using HotelBooking.Tests.Unit.TestDoubles;

namespace HotelBooking.Tests.Unit.Search;

public sealed class SearchHotelsQueryValidatorTests
{
    private readonly SearchHotelsQueryValidator _validator = new(FixedClock.AtToday());

    private static SearchHotelsQuery Valid() =>
        new(null, FixedClock.Today, FixedClock.Today.AddDays(2), 2, 0, 1, null, null, null, [], null, "price", "asc", 1, 20);

    private string[] FailedProperties(SearchHotelsQuery query) =>
        _validator.Validate(query).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    /// <summary>Dates are optional: a search with no dates is a browse.</summary>
    [Fact]
    public void Validate_WithoutDates_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid() with { CheckIn = null, CheckOut = null }).IsValid);

    [Fact]
    public void Validate_CheckInInThePast_HasCheckInError() =>
        Assert.Contains(
            nameof(SearchHotelsQuery.CheckIn),
            FailedProperties(Valid() with { CheckIn = FixedClock.Today.AddDays(-1) }));

    [Fact]
    public void Validate_StayLongerThanTheMaximum_HasCheckOutError()
    {
        var query = Valid() with { CheckOut = FixedClock.Today.AddDays(DateRange.MaxNights + 1) };

        Assert.Contains(nameof(SearchHotelsQuery.CheckOut), FailedProperties(query));
    }

    [Fact]
    public void Validate_StayExactlyAtTheMaximum_HasNoError()
    {
        var query = Valid() with { CheckOut = FixedClock.Today.AddDays(DateRange.MaxNights) };

        Assert.DoesNotContain(nameof(SearchHotelsQuery.CheckOut), FailedProperties(query));
    }

    /// <summary>Every room needs at least one adult in it.</summary>
    [Fact]
    public void Validate_MoreRoomsThanAdults_HasRoomsError() =>
        Assert.Contains(nameof(SearchHotelsQuery.Rooms), FailedProperties(Valid() with { Adults = 1, Rooms = 2 }));

    [Fact]
    public void Validate_MaxPriceBelowMinPrice_HasMaxPriceError() =>
        Assert.Contains(
            nameof(SearchHotelsQuery.MaxPrice),
            FailedProperties(Valid() with { MinPrice = 300m, MaxPrice = 100m }));

    [Fact]
    public void Validate_UnknownSortColumn_HasSortByError() =>
        Assert.Contains(nameof(SearchHotelsQuery.SortBy), FailedProperties(Valid() with { SortBy = "rating" }));

    [Fact]
    public void Validate_UnknownHotelType_HasHotelTypeError() =>
        Assert.Contains(nameof(SearchHotelsQuery.HotelType), FailedProperties(Valid() with { HotelType = "Castle" }));
}