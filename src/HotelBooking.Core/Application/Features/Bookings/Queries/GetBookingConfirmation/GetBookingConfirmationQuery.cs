using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Bookings.Queries.GetBookingConfirmation;

public sealed record GetBookingConfirmationQuery(int BookingId) : IQuery<BookingConfirmationResponse>;