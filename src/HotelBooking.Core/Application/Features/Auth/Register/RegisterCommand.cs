using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone): ICommand<int>;