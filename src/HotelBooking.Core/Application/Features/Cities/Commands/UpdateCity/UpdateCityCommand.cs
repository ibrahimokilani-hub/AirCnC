using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Commands.UpdateCity;

public sealed record UpdateCityCommand(int Id, string Name, string Country, string PostOffice) : ICommand;