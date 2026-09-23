using FluentValidation;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.RoomTypes;

public abstract class RoomTypeValidator<T> : AbstractValidator<T>
    where T : IRoomTypeFields
{
    protected RoomTypeValidator()
    {
        RuleFor(roomType => roomType.Name).NotEmpty().MaximumLength(RoomType.NameMaxLength);
        RuleFor(roomType => roomType.Description).NotNull().MaximumLength(RoomType.DescriptionMaxLength);
        RuleFor(roomType => roomType.PricePerNight).GreaterThan(0).PrecisionScale(18, 2, ignoreTrailingZeros: true);
        RuleFor(roomType => roomType.MaxAdults).InclusiveBetween(1, RoomType.MaxGuestsPerRoom);
        RuleFor(roomType => roomType.MaxChildren).InclusiveBetween(0, RoomType.MaxGuestsPerRoom);
    }
}

public interface IRoomTypeFields
{
    string Name { get; }
    string Description { get; }
    decimal PricePerNight { get; }
    int MaxAdults { get; }
    int MaxChildren { get; }
}