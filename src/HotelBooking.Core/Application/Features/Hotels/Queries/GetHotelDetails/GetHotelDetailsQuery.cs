using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelDetails;

public sealed record GetHotelDetailsQuery(int Id) : IQuery<HotelDetailsResponse>;