using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class DiscountTests
{
    private static readonly DateOnly Start = new(2026, 6, 1);
    private static readonly DateOnly End = new(2026, 6, 30);

    /// <summary>Discount.Create is internal, so a discount is always born through its hotel.</summary>
    private static Discount Summer(
        DiscountType discountType = DiscountType.Percentage,
        decimal value = 10m,
        DateOnly? startsAt = null,
        DateOnly? endsAt = null) =>
        Hotel.Create(1, 1, "Grand", "Nice.", 4, HotelType.Luxury, "1 Main St", 32m, 35m)
            .AddDiscount("Summer", discountType, value, startsAt ?? Start, endsAt ?? End, true);

    [Theory]
    [InlineData(10, 200, 180)]
    [InlineData(90, 100, 10)]
    [InlineData(0, 150, 150)]
    public void ApplyTo_Percentage_TakesThatShareOff(decimal percentage, decimal price, decimal expected) =>
        Assert.Equal(expected, Summer(value: percentage).ApplyTo(price));

    [Fact]
    public void ApplyTo_Percentage_RoundsToTwoDecimals() =>
        Assert.Equal(74.99m, Summer(value: 25m).ApplyTo(99.99m));

    [Theory]
    [InlineData(30, 100, 70)]
    [InlineData(500, 100, 0)]
    public void ApplyTo_FixedAmount_SubtractsAndNeverGoesBelowZero(decimal off, decimal price, decimal expected) =>
        Assert.Equal(expected, Summer(DiscountType.FixedAmount, off).ApplyTo(price));

    [Fact]
    public void ApplyTo_NegativePrice_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Summer().ApplyTo(-1m));

    [Theory]
    [InlineData(2026, 5, 31, false)]
    [InlineData(2026, 6, 1, true)]
    [InlineData(2026, 6, 30, true)]
    [InlineData(2026, 7, 1, false)]
    public void IsActiveOn_CoversBothEndsOfTheWindow(int year, int month, int day, bool expected) =>
        Assert.Equal(expected, Summer().IsActiveOn(new DateOnly(year, month, day)));

    /// <summary>A hotel may not give more than 90% away.</summary>
    [Fact]
    public void Create_PercentageOver90_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Summer(value: 91m));

    [Fact]
    public void Create_EndBeforeStart_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Summer(endsAt: Start.AddDays(-1)));
}