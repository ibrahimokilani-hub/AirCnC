using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Abstractions;

public interface IHotelOwnership
{
    Task<Result> EnsureOwnerAsync(int hotelId, CancellationToken cancellationToken);
}