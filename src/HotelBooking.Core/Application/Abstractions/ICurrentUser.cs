using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Application.Abstractions;

public interface ICurrentUser
{
    int? UserId { get; }
    UserRole? Role { get; }
}