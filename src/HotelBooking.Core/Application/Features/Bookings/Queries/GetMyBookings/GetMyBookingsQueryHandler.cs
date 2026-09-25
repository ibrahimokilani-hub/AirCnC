using FluentValidation;
using HotelBooking.Contracts.Bookings.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Bookings.Queries.GetMyBookings;

public sealed class GetMyBookingsQueryHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    IValidator<GetMyBookingsQuery> validator)
    : IQueryHandler<GetMyBookingsQuery, PagedResult<MyBookingItem>>
{
    public async Task<Result<PagedResult<MyBookingItem>>> HandleAsync(
        GetMyBookingsQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<MyBookingItem>>.Failure(validation.ToValidationError());
        }

        if (currentUser.UserId is not { } userId)
        {
            return Result<PagedResult<MyBookingItem>>.Failure(AuthErrors.NotAuthenticated);
        }

        var page = await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.UserId == userId)
            .OrderByDescending(booking => booking.CreatedAtUtc)
            .Select(booking => new MyBookingItem(
                booking.Id,
                Booking.ConfirmationPrefix + booking.Id.ToString(),
                context.Hotels.IgnoreQueryFilters().Where(h => h.Id == booking.HotelId).Select(h => h.Name).First(),
                context.Hotels.IgnoreQueryFilters().Where(h => h.Id == booking.HotelId).Select(h => h.City.Name).First(),
                booking.Items.Min(item => item.CheckIn),
                booking.Items.Max(item => item.CheckOut),
                booking.Items.Count,
                booking.Status.ToString(),
                booking.TotalPrice,
                booking.CreatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<MyBookingItem>>.Success(page);
    }
}
