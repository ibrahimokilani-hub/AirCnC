using HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;
using HotelBooking.Tests.Unit.TestDoubles;

namespace HotelBooking.Tests.Unit.Bookings;

public sealed class CheckoutCommandValidatorTests
{
    private readonly CheckoutCommandValidator _validator = new(FixedClock.AtToday());

    private static CheckoutCommand Valid() =>
        new(1, FixedClock.Today, FixedClock.Today.AddDays(2), 2, 0, null, Guid.NewGuid());

    private string[] FailedProperties(CheckoutCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    /// <summary>Today is still a legal check-in; yesterday is not.</summary>
    [Fact]
    public void Validate_CheckInInThePast_HasCheckInError()
    {
        var command = Valid() with { CheckIn = FixedClock.Today.AddDays(-1) };

        Assert.Contains(nameof(CheckoutCommand.CheckIn), FailedProperties(command));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_CheckOutNotAfterCheckIn_HasCheckOutError(int checkOutOffset)
    {
        var command = Valid() with { CheckOut = FixedClock.Today.AddDays(checkOutOffset) };

        Assert.Contains(nameof(CheckoutCommand.CheckOut), FailedProperties(command));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validate_AdultsOutOfRange_HasAdultsError(int adults)
    {
        Assert.Contains(nameof(CheckoutCommand.Adults), FailedProperties(Valid() with { Adults = adults }));
    }

    [Fact]
    public void Validate_NegativeChildren_HasChildrenError() =>
        Assert.Contains(nameof(CheckoutCommand.Children), FailedProperties(Valid() with { Children = -1 }));

    /// <summary>The key is what makes a retried checkout safe, so it is required.</summary>
    [Fact]
    public void Validate_WithoutAnIdempotencyKey_HasKeyError() =>
        Assert.Contains(
            nameof(CheckoutCommand.IdempotencyKey),
            FailedProperties(Valid() with { IdempotencyKey = Guid.Empty }));
}