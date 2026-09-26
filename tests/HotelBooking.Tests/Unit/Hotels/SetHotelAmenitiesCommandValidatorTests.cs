using HotelBooking.Core.Application.Features.Hotels.Commands.SetHotelAmenities;

namespace HotelBooking.Tests.Unit.Hotels;

public sealed class SetHotelAmenitiesCommandValidatorTests
{
    private readonly SetHotelAmenitiesCommandValidator _validator = new();

    [Fact]
    public void Validate_AListOfRealIds_HasNoErrors() =>
        Assert.True(_validator.Validate(new SetHotelAmenitiesCommand(1, [1, 2, 3])).IsValid);

    /// <summary>An empty list is how a hotel clears its amenities.</summary>
    [Fact]
    public void Validate_AnEmptyList_HasNoErrors() =>
        Assert.True(_validator.Validate(new SetHotelAmenitiesCommand(1, [])).IsValid);

    [Fact]
    public void Validate_AnIdThatCannotExist_HasAnError()
    {
        var result = _validator.Validate(new SetHotelAmenitiesCommand(1, [1, 0]));

        Assert.False(result.IsValid);
        Assert.Contains("AmenityIds[1]", result.Errors.Select(error => error.PropertyName));
    }
}