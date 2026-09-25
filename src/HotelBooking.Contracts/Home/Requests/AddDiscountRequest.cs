namespace HotelBooking.Contracts.Home.Requests;

public sealed record AddDiscountRequest(string Name, string DiscountType, decimal Value, DateOnly StartsAt, DateOnly EndsAt);