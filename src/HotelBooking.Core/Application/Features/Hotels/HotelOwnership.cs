using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels;

public sealed class HotelOwnership(IAppDbContext context, ICurrentUser currentUser) : IHotelOwnership
{
    public async Task<Result> EnsureOwnerAsync(int hotelId, CancellationToken cancellationToken)
    {
        var ownerId = await context.Hotels
            .AsNoTracking()
            .Where(hotel => hotel.Id == hotelId)
            .Select(hotel => (int?)hotel.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ownerId is null)
        {
            return Result.Failure(HotelErrors.NotFound(hotelId));
        }

        return ownerId == currentUser.UserId
            ? Result.Success()
            : Result.Failure(HotelErrors.NotYours(hotelId));
    }
}