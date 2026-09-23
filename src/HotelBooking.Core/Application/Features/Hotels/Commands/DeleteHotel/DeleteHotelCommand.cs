using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.DeleteHotel;

public sealed record DeleteHotelCommand(int Id) : ICommand;