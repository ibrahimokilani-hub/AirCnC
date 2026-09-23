using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddNearbyAttraction;

public sealed record AddNearbyAttractionCommand(
    int HotelId,
    string Name,
    string Category,
    decimal Latitude,
    decimal Longitude)
    : ICommand<int>;