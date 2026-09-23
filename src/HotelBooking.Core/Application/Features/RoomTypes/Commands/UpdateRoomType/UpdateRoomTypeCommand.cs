using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.UpdateRoomType;

public sealed record UpdateRoomTypeCommand(
    int HotelId,
    int Id,
    string Name,
    string Description,
    decimal PricePerNight,
    int MaxAdults,
    int MaxChildren)
    : ICommand, IRoomTypeFields;