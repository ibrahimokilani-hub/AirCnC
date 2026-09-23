using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelById;

public sealed record GetHotelByIdQuery(int Id) : IQuery<HotelResponse>;