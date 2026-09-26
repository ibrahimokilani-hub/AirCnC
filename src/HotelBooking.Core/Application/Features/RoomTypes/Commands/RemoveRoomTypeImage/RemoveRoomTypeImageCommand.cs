using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.RemoveRoomTypeImage;

public sealed record RemoveRoomTypeImageCommand(int HotelId, int RoomTypeId, int ImageId) : ICommand;
