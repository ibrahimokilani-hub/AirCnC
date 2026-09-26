using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Amenities.Commands.CreateAmenity;

public sealed record CreateAmenityCommand(string Name, string? AmenityImageUrl) : ICommand<int>;