namespace HotelBooking.Contracts.Common;

public sealed record ApiResponse<T>(T Data);
