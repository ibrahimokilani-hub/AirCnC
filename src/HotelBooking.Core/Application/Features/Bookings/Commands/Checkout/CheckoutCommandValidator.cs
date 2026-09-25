using FluentValidation;

namespace HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;

public sealed class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(command => command.RoomTypeId).GreaterThan(0);

        RuleFor(command => command.CheckIn)
            .GreaterThanOrEqualTo(today).WithMessage("Check-in can't be in the past");

        RuleFor(command => command.CheckOut)
            .GreaterThan(command => command.CheckIn).WithMessage("Check-out must be after check-in");

        RuleFor(command => command.Adults).InclusiveBetween(1, 20);
        RuleFor(command => command.Children).InclusiveBetween(0, 20);

        RuleFor(command => command.IdempotencyKey)
            .NotEmpty().WithMessage("Send the Idempotency-Key header your checkout started with.");

        RuleFor(command => command.Notes).MaximumLength(2500);
    }
}
