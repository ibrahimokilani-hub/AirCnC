namespace HotelBooking.Contracts.Common;

public sealed record ImageRequest(string ImageUrl);

public sealed record ImageResponse(int Id, string ImageUrl);
