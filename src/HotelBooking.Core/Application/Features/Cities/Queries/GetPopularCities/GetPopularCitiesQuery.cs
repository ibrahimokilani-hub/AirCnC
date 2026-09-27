using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetPopularCities;

public sealed record GetPopularCitiesQuery: IQuery<IReadOnlyList<CityBookingCountResponse>>;