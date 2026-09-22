using HotelBooking.Contracts.Auth.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password): ICommand<AuthResponse>;