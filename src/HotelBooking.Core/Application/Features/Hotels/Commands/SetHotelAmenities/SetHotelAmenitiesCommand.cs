using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.SetHotelAmenities;

public sealed record SetHotelAmenitiesCommand(int HotelId, IReadOnlyList<int> AmenityIds) : ICommand;