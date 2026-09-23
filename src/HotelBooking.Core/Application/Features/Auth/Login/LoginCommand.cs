using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password): ICommand<AuthTokens>;