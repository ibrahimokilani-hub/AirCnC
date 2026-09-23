using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Rooms;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Rooms.Queries.GetRoomsList;

public sealed class GetRoomsListQueryHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<GetRoomsListQuery> validator)
    : IQueryHandler<GetRoomsListQuery, PagedResult<RoomListItem>>
{
    public async Task<Result<PagedResult<RoomListItem>>> HandleAsync(
        GetRoomsListQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<RoomListItem>>.Failure(validation.ToValidationError());
        }

        var access = await ownership.EnsureOwnerAsync(query.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return Result<PagedResult<RoomListItem>>.Failure(access.Error!);
        }

        var rooms = context.Rooms.AsNoTracking().Where(room => room.HotelId == query.HotelId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            rooms = rooms.Where(room => room.Number.Contains(search));
        }

        var page = await Sort(rooms, query.SortBy, query.SortDirection)
            .Select(room => new RoomListItem(
                room.Id,
                room.Number,
                room.RoomTypeId,
                room.RoomType.Name,
                room.RoomType.MaxAdults,
                room.RoomType.MaxChildren,
                room.CreatedAtUtc,
                room.UpdatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<RoomListItem>>.Success(page);
    }

    private static IOrderedQueryable<Room> Sort(IQueryable<Room> rooms, string? sortBy, string? direction)
    {
        var descending = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        var sorted = sortBy?.ToLowerInvariant() switch
        {
            null or "number" => descending ? rooms.OrderByDescending(r => r.Number) : rooms.OrderBy(r => r.Number),
            "roomtype" => descending ? rooms.OrderByDescending(r => r.RoomType.Name) : rooms.OrderBy(r => r.RoomType.Name),
            "createdat" => descending ? rooms.OrderByDescending(r => r.CreatedAtUtc) : rooms.OrderBy(r => r.CreatedAtUtc),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, "The validator should have rejected this.")
        };

        return sorted.ThenBy(r => r.Id);
    }
}
