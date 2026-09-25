using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Application.Features.Hotels.Discounts.Commands;

public sealed record AddDiscountCommand(int HotelId, string Name, string DiscountType, decimal Value, DateOnly StartsAt, DateOnly EndsAt) : ICommand<int>;
