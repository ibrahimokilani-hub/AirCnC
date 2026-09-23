using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.DeleteRoom;

public sealed record DeleteRoomCommand(int HotelId, int Id) : ICommand;