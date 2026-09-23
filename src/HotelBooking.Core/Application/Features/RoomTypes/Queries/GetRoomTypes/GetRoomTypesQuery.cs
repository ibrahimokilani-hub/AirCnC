using HotelBooking.Contracts.RoomTypes.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.RoomTypes.Queries.GetRoomTypes;

public sealed record GetRoomTypesQuery(int HotelId) : IQuery<IReadOnlyList<RoomTypeResponse>>;