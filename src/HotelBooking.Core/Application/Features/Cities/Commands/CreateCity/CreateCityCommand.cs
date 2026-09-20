using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

public sealed record CreateCityCommand(string Name, string Country, string PostOffice)
    : ICommand<int>;