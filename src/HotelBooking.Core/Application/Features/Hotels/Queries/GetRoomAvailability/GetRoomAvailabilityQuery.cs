using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetRoomAvailability;

public sealed record GetRoomAvailabilityQuery(
    int HotelId,
    DateOnly? CheckIn,
    DateOnly? CheckOut,
    int Adults,
    int Children)
    : IQuery<IReadOnlyList<RoomTypeAvailabilityResponse>>;