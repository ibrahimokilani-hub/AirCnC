using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.CreateRoom;

public sealed record CreateRoomCommand(int HotelId, string Number, int RoomTypeId) : ICommand<int>;