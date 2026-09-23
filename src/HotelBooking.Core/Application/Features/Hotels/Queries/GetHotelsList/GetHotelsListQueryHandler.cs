using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelsList;

public sealed class GetHotelsListQueryHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    IValidator<GetHotelsListQuery> validator)
    : IQueryHandler<GetHotelsListQuery, PagedResult<HotelListItem>>
{
    public async Task<Result<PagedResult<HotelListItem>>> HandleAsync(
        GetHotelsListQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<HotelListItem>>.Failure(validation.ToValidationError());
        }
        
        IQueryable<Hotel> hotels = context.Hotels
            .AsNoTracking()
            .Where(hotel => hotel.OwnerId == currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            hotels = hotels.Where(hotel =>
                hotel.Name.Contains(search) ||
                hotel.Owner.FirstName.Contains(search) ||
                hotel.Owner.LastName.Contains(search) ||
                hotel.City.Name.Contains(search));
        }

        var page = await Sort(hotels, query.SortBy, query.SortDirection)
            .Select(hotel => new HotelListItem(
                hotel.Id,
                hotel.Name,
                hotel.City.Name,
                hotel.StarRating,
                hotel.Owner.FirstName + " " + hotel.Owner.LastName,
                hotel.HotelType.ToString(),
                hotel.CreatedAtUtc,
                hotel.UpdatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<HotelListItem>>.Success(page);
    }

    private static IOrderedQueryable<Hotel> Sort(IQueryable<Hotel> hotels, string? sortBy, string? direction)
    {
        var descending = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        var sorted = sortBy?.ToLowerInvariant() switch
        {
            null or "name" => descending ? hotels.OrderByDescending(h => h.Name) : hotels.OrderBy(h => h.Name),
            "city" => descending ? hotels.OrderByDescending(h => h.City.Name) : hotels.OrderBy(h => h.City.Name),
            "starrating" => descending ? hotels.OrderByDescending(h => h.StarRating) : hotels.OrderBy(h => h.StarRating),
            "createdat" => descending ? hotels.OrderByDescending(h => h.CreatedAtUtc) : hotels.OrderBy(h => h.CreatedAtUtc),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, "The validator should have rejected this.")
        };

        return sorted.ThenBy(h => h.Id);
    }
}
