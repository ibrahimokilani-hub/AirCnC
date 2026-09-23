using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Rooms.Commands.UpdateRoom;

public sealed record UpdateRoomCommand(int HotelId, int Id, string Number, int RoomTypeId) : ICommand;