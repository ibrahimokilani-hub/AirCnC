using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.DeleteRoomType;

public sealed record DeleteRoomTypeCommand(int HotelId, int Id) : ICommand;