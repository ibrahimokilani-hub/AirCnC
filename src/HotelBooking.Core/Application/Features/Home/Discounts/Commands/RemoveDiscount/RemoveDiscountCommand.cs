using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Discounts.Commands.RemoveDiscount;

public sealed record RemoveDiscountCommand(int HotelId, int DiscountId) : ICommand;
