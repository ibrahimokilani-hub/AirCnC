namespace HotelBooking.Core.Application.Common.Paging;

public static class ListRules
{
    public const int MaxPageSize = 100;
    public const int MaxSearchLength = 100;

    public static readonly string[] SortDirections = ["asc", "desc"];
}