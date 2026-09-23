using HotelBooking.Contracts.RoomTypes;
using HotelBooking.Contracts.RoomTypes.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Queries.GetRoomTypes;

public sealed class GetRoomTypesQueryHandler(IAppDbContext context, IHotelOwnership ownership)
    : IQueryHandler<GetRoomTypesQuery, IReadOnlyList<RoomTypeResponse>>
{
    public async Task<Result<IReadOnlyList<RoomTypeResponse>>> HandleAsync(
        GetRoomTypesQuery query,
        CancellationToken cancellationToken)
    {
        var access = await ownership.EnsureOwnerAsync(query.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return Result<IReadOnlyList<RoomTypeResponse>>.Failure(access.Error!);
        }

        var roomTypes = await context.RoomTypes
            .AsNoTracking()
            .Where(roomType => roomType.HotelId == query.HotelId)
            .OrderBy(roomType => roomType.PricePerNight)
            .ThenBy(roomType => roomType.Id)
            .Select(roomType => new RoomTypeResponse(
                roomType.Id,
                roomType.Name,
                roomType.Description,
                roomType.PricePerNight,
                roomType.MaxAdults,
                roomType.MaxChildren))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<RoomTypeResponse>>.Success(roomTypes);
    }
}