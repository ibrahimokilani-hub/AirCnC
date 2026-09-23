namespace HotelBooking.Contracts.Search;

public sealed class SearchHotelsRequest
{
    public string? Query { get; init; }

    public DateOnly? CheckIn { get; init; }

    public DateOnly? CheckOut { get; init; }

    public int Adults { get; init; } = 2;

    public int Children { get; init; }

    public int Rooms { get; init; } = 1;

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public int? MinStars { get; init; }

    public List<int> AmenityIds { get; init; } = [];

    public string? HotelType { get; init; }

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}