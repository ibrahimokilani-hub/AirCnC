using HotelBooking.Contracts.Home;
using HotelBooking.Contracts.Home.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HotelBooking.Core.Application.Features.Home.Discounts.Queries.GetFeaturedDeals;

public sealed class GetFeaturedDealsQueryHandler(
    IAppDbContext context,
    IDistributedCache cache,
    TimeProvider timeProvider)
    : IQueryHandler<GetFeaturedDealsQuery, IReadOnlyList<FeaturedDealResponse>>
{
    private static readonly TimeSpan CacheFor = TimeSpan.FromMinutes(5);

    public async Task<Result<IReadOnlyList<FeaturedDealResponse>>> HandleAsync(
        GetFeaturedDealsQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        // the third [arg], is when the cache miss happens on the featured deals...
        var deals = await cache.GetOrCreateAsync(
            CacheKeys.FeaturedDeals(today),
            CacheFor,
            token => LoadAsync(today, token),
            cancellationToken);

        return Result<IReadOnlyList<FeaturedDealResponse>>.Success(deals);
    }

    private async Task<FeaturedDealResponse[]> LoadAsync(
    DateOnly today,
    CancellationToken cancellationToken)
{
    var candidates = await context.Hotels
        .AsNoTracking()
        .Where(hotel => hotel.RoomTypes.Any())
        .Select(hotel => new
        {
            hotel.Id,
            hotel.Name,
            CityName = hotel.City.Name,
            hotel.City.Country,
            hotel.StarRating,
            hotel.HotelImageUrl,
            FromPrice = hotel.RoomTypes.Min(roomType => roomType.PricePerNight),
            Best = hotel.Discounts
                .Where(discount => discount.IsActive && discount.StartsAt <= today && today <= discount.EndsAt)                .OrderByDescending(discount =>
                    discount.DiscountType == DiscountType.Percentage
                        ? discount.Value
                        : discount.Value / hotel.RoomTypes.Min(roomType => roomType.PricePerNight) * 100m)
                .Select(discount => new
                {
                    discount.Name,
                    discount.DiscountType,
                    discount.Value,
                    discount.EndsAt
                })
                .FirstOrDefault()
        })
        .Where(hotel => hotel.Best != null)
        .ToListAsync(cancellationToken);

    return candidates
        .Select(hotel =>
        {
            var originalPrice = hotel.FromPrice;

            var discountedPrice = hotel.Best!.DiscountType switch
            {
                DiscountType.Percentage =>
                    originalPrice * (100m - hotel.Best.Value) / 100m,

                DiscountType.FixedAmount =>
                    Math.Max(originalPrice - hotel.Best.Value, 0m),

                _ => throw new ArgumentOutOfRangeException()
            };

            return new FeaturedDealResponse(
                hotel.Id,
                hotel.Name,
                hotel.CityName,
                hotel.Country,
                hotel.StarRating,
                hotel.Best.Name,
                hotel.Best.DiscountType.ToString(),
                hotel.Best.Value,
                hotel.HotelImageUrl,
                originalPrice,
                decimal.Round(discountedPrice, 2),
                hotel.Best.EndsAt);
        })
        .OrderByDescending(deal =>
            (deal.OriginalPrice - deal.DiscountedPrice) / deal.OriginalPrice)
        .ThenByDescending(deal => deal.StarRating)
        .Take(GetFeaturedDealsQuery.Count)
        .ToArray();
}
}
