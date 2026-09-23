using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Rooms;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Rooms.Queries.GetRoomsList;

public sealed record GetRoomsListQuery(
    int HotelId,
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    string? SortDirection)
    : IQuery<PagedResult<RoomListItem>>
{
    public static readonly string[] SortableColumns = ["number", "roomType", "createdAt"];
}