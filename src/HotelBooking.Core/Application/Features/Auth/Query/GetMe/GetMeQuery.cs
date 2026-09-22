using HotelBooking.Contracts.Auth.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Query.GetMe;

public sealed record GetMeQuery : IQuery<LoggedInUserResponse>;