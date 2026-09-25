namespace HotelBooking.Core.Application.Abstractions;

public static class CacheKeys
{
    public static string FeaturedDeals(DateOnly day) => $"home:featured-deals:{day:yyyyMMdd}";

    public const string TrendingDestinations = "home:trending-destinations";
}