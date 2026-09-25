using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Bookings.Queries.GetMyBookings;

public sealed record GetMyBookingsQuery(int Page, int PageSize) : IQuery<PagedResult<MyBookingItem>>;