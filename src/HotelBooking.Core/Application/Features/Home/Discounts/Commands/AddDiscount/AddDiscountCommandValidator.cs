using FluentValidation;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Application.Features.Hotels.Discounts.Commands;

public sealed class AddDiscountCommandValidator : AbstractValidator<AddDiscountCommand>
{
    public AddDiscountCommandValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        
        RuleFor(command => command.HotelId).GreaterThan(0);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Discount.NameMaxLength);

        RuleFor(command => command.DiscountType)
            .NotEmpty()
            .IsEnumName(typeof(DiscountType), caseSensitive: false)
            .WithMessage($"Must be one of: {string.Join(", ", Enum.GetNames<DiscountType>())}.");   
        
        RuleFor(command => command.EndsAt)
            .GreaterThanOrEqualTo(command => command.StartsAt).WithMessage("A discount must end on or after it starts.")
            .GreaterThanOrEqualTo(today).WithMessage("A discount that has already ended would never show.");
    }
}
