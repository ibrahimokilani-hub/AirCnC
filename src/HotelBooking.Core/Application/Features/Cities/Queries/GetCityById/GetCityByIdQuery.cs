using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCityById;

public sealed record GetCityByIdQuery(int Id) : IQuery<CityResponse>;