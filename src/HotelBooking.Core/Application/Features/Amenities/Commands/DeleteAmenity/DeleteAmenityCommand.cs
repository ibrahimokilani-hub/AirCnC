using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Amenities.Commands.DeleteAmenity;

public sealed record DeleteAmenityCommand(int Id) : ICommand;