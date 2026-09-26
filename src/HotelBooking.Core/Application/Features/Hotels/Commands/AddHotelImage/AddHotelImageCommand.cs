using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddHotelImage;

public sealed record AddHotelImageCommand(int HotelId, string ImageUrl) : ICommand<int>;
