using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string? RefreshToken) : ICommand<AccessToken>;