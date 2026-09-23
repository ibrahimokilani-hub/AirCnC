using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Search;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Availability;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Application.Features.Search.SearchHotels.Queries;

public sealed class SearchHotelsQueryHandler(
    IAppDbContext context,
    IValidator<SearchHotelsQuery> validator,
    TimeProvider timeProvider)
    : IQueryHandler<SearchHotelsQuery, PagedResult<SearchHotelItem>>
{
    private const int ShortDescriptionLength = 160;

    public async Task<Result<PagedResult<SearchHotelItem>>> HandleAsync(
        SearchHotelsQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<SearchHotelItem>>.Failure(validation.ToValidationError());
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var checkIn = query.CheckIn ?? today;
        var stay = DateRange.Create(checkIn, query.CheckOut ?? checkIn.AddDays(1));

        var adultsPerRoom = (int)Math.Ceiling(query.Adults / (double)query.Rooms);
        var childrenPerRoom = (int)Math.Ceiling(query.Children / (double)query.Rooms);

        var bookableRooms = context.Rooms
            .Where(room => room.RoomType.MaxAdults >= adultsPerRoom && room.RoomType.MaxChildren >= childrenPerRoom)
            .Where(RoomAvailability.IsFreeDuring(stay));

      var candidates = context.Hotels
            .Select(hotel => new
            {
                Hotel = hotel,
                AvailableRooms = bookableRooms.Count(room => room.HotelId == hotel.Id),
                CheapestPrice = bookableRooms
                    .Where(room => room.HotelId == hotel.Id)
                    .Min(room => (decimal?)room.RoomType.PricePerNight)
            })
            .Where(candidate => candidate.AvailableRooms >= query.Rooms);

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var text = query.Query.Trim();
            candidates = candidates.Where(candidate =>
                candidate.Hotel.Name.Contains(text) ||
                candidate.Hotel.City.Name.Contains(text) ||
                candidate.Hotel.City.Country.Contains(text));
        }

        if (query.MinPrice is { } minPrice)
        {
            candidates = candidates.Where(candidate => candidate.CheapestPrice >= minPrice);
        }

        if (query.MaxPrice is { } maxPrice)
        {
            candidates = candidates.Where(candidate => candidate.CheapestPrice <= maxPrice);
        }

        if (query.MinStars is { } minStars)
        {
            candidates = candidates.Where(candidate => candidate.Hotel.StarRating >= minStars);
        }

        if (query.HotelType is not null)
        {
            var hotelType = Enum.Parse<HotelType>(query.HotelType, ignoreCase: true);
            candidates = candidates.Where(candidate => candidate.Hotel.HotelType == hotelType);
        }

        if (query.AmenityIds.Count > 0)
        {
            var amenityIds = query.AmenityIds.Distinct().ToList();
            candidates = candidates.Where(candidate =>
                candidate.Hotel.Amenities.Count(amenity => amenityIds.Contains(amenity.Id)) == amenityIds.Count);
        }

        var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        var sorted = query.SortBy?.ToLowerInvariant() switch
        {
            null or "price" => descending
                ? candidates.OrderByDescending(c => c.CheapestPrice)
                : candidates.OrderBy(c => c.CheapestPrice),
            "stars" => descending
                ? candidates.OrderByDescending(c => c.Hotel.StarRating)
                : candidates.OrderBy(c => c.Hotel.StarRating),
            "name" => descending
                ? candidates.OrderByDescending(c => c.Hotel.Name)
                : candidates.OrderBy(c => c.Hotel.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(query), query.SortBy, "The validator should have rejected this.")
        };

        var page = await sorted
            .ThenBy(candidate => candidate.Hotel.Id)
            .Select(candidate => new SearchHotelItem(
                candidate.Hotel.Id,
                candidate.Hotel.Name,
                candidate.Hotel.City.Name,
                candidate.Hotel.City.Country,
                candidate.Hotel.StarRating,
                candidate.Hotel.HotelType.ToString(),
                candidate.Hotel.Description.Length > ShortDescriptionLength
                    ? candidate.Hotel.Description.Substring(0, ShortDescriptionLength) + "…"
                    : candidate.Hotel.Description,
                candidate.CheapestPrice!.Value,
                candidate.AvailableRooms))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<SearchHotelItem>>.Success(page);
    }
}
