using HotelBooking.Contracts.Amenities;
using HotelBooking.Contracts.Amenities.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Amenities.Queries.GetAmenities;

public sealed record GetAmenitiesQuery : IQuery<IReadOnlyList<AmenityResponse>>;