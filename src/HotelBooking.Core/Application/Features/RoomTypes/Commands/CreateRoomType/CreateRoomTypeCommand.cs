using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.CreateRoomType;

public sealed record CreateRoomTypeCommand(
    int HotelId,
    string Name,
    string Description,
    decimal PricePerNight,
    int MaxAdults,
    int MaxChildren)
    : ICommand<int>, IRoomTypeFields;