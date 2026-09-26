using HotelBooking.Core.Application.Features.RoomTypes.Commands.CreateRoomType;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.RoomTypes;

/// <summary>
/// Create and Update share one RoomTypeValidator&lt;T&gt; base, so these rules are tested once
/// through the create command.
/// </summary>
public sealed class CreateRoomTypeCommandValidatorTests
{
    private readonly CreateRoomTypeCommandValidator _validator = new();

    private static CreateRoomTypeCommand Valid() => new(1, "Standard", "Two beds.", 120m, 2, 1);

    private string[] FailedProperties(CreateRoomTypeCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_BlankName_HasNameError(string name) =>
        Assert.Contains(nameof(CreateRoomTypeCommand.Name), FailedProperties(Valid() with { Name = name }));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PriceNotAboveZero_HasPriceError(decimal pricePerNight) =>
        Assert.Contains(
            nameof(CreateRoomTypeCommand.PricePerNight),
            FailedProperties(Valid() with { PricePerNight = pricePerNight }));

    /// <summary>Money has two decimals; a third would be lost on the way to the database.</summary>
    [Fact]
    public void Validate_PriceWithMoreThanTwoDecimals_HasPriceError() =>
        Assert.Contains(
            nameof(CreateRoomTypeCommand.PricePerNight),
            FailedProperties(Valid() with { PricePerNight = 120.125m }));

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Validate_AdultsOutsideOneToTen_HasAdultsError(int maxAdults) =>
        Assert.Contains(
            nameof(CreateRoomTypeCommand.MaxAdults),
            FailedProperties(Valid() with { MaxAdults = maxAdults }));

    [Fact]
    public void Validate_MaxChildrenAtTheLimit_HasNoError() =>
        Assert.DoesNotContain(
            nameof(CreateRoomTypeCommand.MaxChildren),
            FailedProperties(Valid() with { MaxChildren = RoomType.MaxGuestsPerRoom }));
}