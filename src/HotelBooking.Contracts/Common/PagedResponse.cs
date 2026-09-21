namespace HotelBooking.Contracts.Common;

public sealed record PagedResponse<T>(IReadOnlyList<T> Data, PageMeta Meta);

public sealed record PageMeta(int Page, int PageSize, int TotalCount, int TotalPages);
