using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Rooms;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Availability;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Rooms.Queries.GetRoomsList;

public sealed class GetRoomsListQueryHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    TimeProvider timeProvider,
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

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var isFreeTonight = RoomAvailability.IsFreeDuring(DateRange.Create(today, today.AddDays(1)));

        var rooms = context.Rooms.AsNoTracking().Where(room => room.HotelId == query.HotelId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            rooms = rooms.Where(room => room.Number.Contains(search));
        }

        // The specification is an Expression<Func<Room, bool>>. To use it inside a
        // Select, the free rooms are the rooms that match it; "is this one of them" is
        // then a correlated EXISTS, still one statement.
        var freeTonight = context.Rooms.Where(isFreeTonight);

        var page = await Sort(rooms, query.SortBy, query.SortDirection)
            .Select(room => new RoomListItem(
                room.Id,
                room.Number,
                room.RoomTypeId,
                room.RoomType.Name,
                room.RoomType.MaxAdults,
                room.RoomType.MaxChildren,
                freeTonight.Any(free => free.Id == room.Id),
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
