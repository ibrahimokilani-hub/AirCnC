using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.RemoveHotelImage;

public sealed record RemoveHotelImageCommand(int HotelId, int ImageId) : ICommand;
