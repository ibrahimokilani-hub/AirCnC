using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.RemoveNearbyAttraction;

public sealed record RemoveNearbyAttractionCommand(int HotelId, int Id) : ICommand;