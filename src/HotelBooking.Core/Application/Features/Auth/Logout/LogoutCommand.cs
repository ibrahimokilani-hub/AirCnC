using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken) : ICommand;