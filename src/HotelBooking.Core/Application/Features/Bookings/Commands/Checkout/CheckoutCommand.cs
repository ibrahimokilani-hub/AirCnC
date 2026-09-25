using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Bookings.Commands.Checkout;

public sealed record CheckoutCommand(
    int RoomTypeId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Adults,
    int Children,
    string? Notes,
    Guid IdempotencyKey)
    : ICommand<CheckoutResponse>;
