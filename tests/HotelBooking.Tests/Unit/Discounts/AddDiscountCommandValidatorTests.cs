using HotelBooking.Core.Application.Features.Home.Discounts.Commands.AddDiscount;
using HotelBooking.Core.Application.Features.Hotels.Discounts.Commands;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Tests.Unit.TestDoubles;

namespace HotelBooking.Tests.Unit.Discounts;

public sealed class AddDiscountCommandValidatorTests
{
    private readonly AddDiscountCommandValidator _validator = new(FixedClock.AtToday());

    private static AddDiscountCommand Valid() =>
        new(1, "Summer", nameof(DiscountType.Percentage), 15m, FixedClock.Today, FixedClock.Today.AddDays(30), true);

    private string[] FailedProperties(AddDiscountCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    [Fact]
    public void Validate_EndBeforeStart_HasEndsAtError()
    {
        var command = Valid() with { EndsAt = FixedClock.Today.AddDays(-1) };

        Assert.Contains(nameof(AddDiscountCommand.EndsAt), FailedProperties(command));
    }

    /// <summary>A window that closed before today would never show on the home page.</summary>
    [Fact]
    public void Validate_AlreadyFinished_HasEndsAtError()
    {
        var command = Valid() with
        {
            StartsAt = FixedClock.Today.AddDays(-10),
            EndsAt = FixedClock.Today.AddDays(-5)
        };

        Assert.Contains(nameof(AddDiscountCommand.EndsAt), FailedProperties(command));
    }

    [Fact]
    public void Validate_UnknownDiscountType_HasDiscountTypeError() =>
        Assert.Contains(
            nameof(AddDiscountCommand.DiscountType),
            FailedProperties(Valid() with { DiscountType = "HalfPrice" }));

    [Fact]
    public void Validate_NegativeValue_HasValueError() =>
        Assert.Contains(nameof(AddDiscountCommand.Value), FailedProperties(Valid() with { Value = -1m }));
}