using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Discount : Entity
{
    
    public const int NameMaxLength = 200;
    
    private Discount(
        string name,
        DiscountType discountType,
        decimal value,
        DateOnly startsAt,
        DateOnly endsAt)
    {
        Name = name;
        DiscountType = discountType;
        Value = value;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public int HotelId { get; private set; }

    public string Name { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal Value { get; private set; }

    public DateOnly StartsAt { get; private set; }

    public DateOnly EndsAt { get; private set; }

    public bool IsActiveOn(DateOnly day) =>
        StartsAt <= day && day <= EndsAt;

    public decimal ApplyTo(decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        var discountedPrice = DiscountType switch
        {
            DiscountType.Percentage =>
                price * (100m - Value) / 100m,

            DiscountType.FixedAmount =>
                price - Value,

            _ => throw new ArgumentOutOfRangeException(nameof(DiscountType))
        };

        return decimal.Round(Math.Max(discountedPrice, 0m), 2);
    }

    internal static Discount Create(
        string name,
        DiscountType discountType,
        decimal value,
        DateOnly startsAt,
        DateOnly endsAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0m);
        ArgumentOutOfRangeException.ThrowIfLessThan(endsAt, startsAt);

        if (discountType == DiscountType.Percentage)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 90m);
        }

        return new Discount(
            name,
            discountType,
            value,
            startsAt,
            endsAt);
    }
}