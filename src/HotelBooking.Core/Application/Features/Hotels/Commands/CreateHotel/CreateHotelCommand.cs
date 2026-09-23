using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.CreateHotel;

public sealed record CreateHotelCommand(
    int CityId,
    string Name,
    string Description,
    int StarRating,
    string HotelType,
    string Address,
    decimal Latitude,
    decimal Longitude)
    : ICommand<int>;
