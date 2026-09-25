namespace HotelBooking.Contracts.Home.Responses;

public sealed record FeaturedDealResponse(
    int HotelId,
    string HotelName,
    string CityName,
    string Country,
    int StarRating,
    string DiscountName,
    string DiscountType,
    decimal DiscountedValue,
    decimal OriginalPrice,
    decimal DiscountedPrice,
    DateOnly EndsAt);