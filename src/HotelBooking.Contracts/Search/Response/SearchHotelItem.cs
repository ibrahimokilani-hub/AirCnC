namespace HotelBooking.Contracts.Search;

public sealed record SearchHotelItem(
    int Id,
    string Name,
    string CityName,
    string Country,
    int StarRating,
    string HotelType,
    string ShortDescription,
    decimal PricePerNight,
    int AvailableRooms);