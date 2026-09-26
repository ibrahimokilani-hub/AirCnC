using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.AddRoomTypeImage;

public sealed record AddRoomTypeImageCommand(int HotelId, int RoomTypeId, string ImageUrl) : ICommand<int>;
