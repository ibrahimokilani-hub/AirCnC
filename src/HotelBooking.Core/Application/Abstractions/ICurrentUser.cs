namespace HotelBooking.Core.Application.Abstractions;

public interface ICurrentUser
{
    int? UserId { get; }
}