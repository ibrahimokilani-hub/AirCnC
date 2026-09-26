namespace HotelBooking.Contracts.Reviews.Responses;


public sealed record ReviewResponse(int Id, int Rating, string ReviewerName, string Comment, DateTime CreatedAtUtc);