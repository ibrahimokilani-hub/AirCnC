using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Commands.DeleteCity;

public sealed record DeleteCityCommand(int Id) : ICommand;