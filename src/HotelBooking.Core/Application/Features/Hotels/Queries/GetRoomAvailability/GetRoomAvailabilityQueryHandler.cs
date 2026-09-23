using FluentValidation;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Availability;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;

public sealed class GetRoomAvailabilityQueryHandler(
    IAppDbContext context,
    IValidator<GetRoomAvailabilityQuery> validator,
    TimeProvider timeProvider)
    : IQueryHandler<GetRoomAvailabilityQuery, IReadOnlyList<RoomTypeAvailabilityResponse>>
{
    public async Task<Result<IReadOnlyList<RoomTypeAvailabilityResponse>>> HandleAsync(
        GetRoomAvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>.Failure(validation.ToValidationError());
        }

        if (!await context.Hotels.AnyAsync(hotel => hotel.Id == query.HotelId, cancellationToken))
        {
            return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>.Failure(HotelErrors.NotFound(query.HotelId));
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var checkIn = query.CheckIn ?? today;
        var stay = DateRange.Create(checkIn, query.CheckOut ?? checkIn.AddDays(1));

        var freeRooms = context.Rooms.Where(RoomAvailability.IsFreeDuring(stay));

        var roomTypes = await context.RoomTypes
            .AsNoTracking()
            .Where(roomType => roomType.HotelId == query.HotelId)
            .OrderBy(roomType => roomType.PricePerNight)
            .ThenBy(roomType => roomType.Id)
            .Select(roomType => new RoomTypeAvailabilityResponse(
                roomType.Id,
                roomType.Name,
                roomType.Description,
                roomType.PricePerNight,
                roomType.MaxAdults,
                roomType.MaxChildren,
                freeRooms.Count(room => room.RoomTypeId == roomType.Id),
                roomType.MaxAdults >= query.Adults && roomType.MaxChildren >= query.Children))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<RoomTypeAvailabilityResponse>>.Success(roomTypes);
    }
}
