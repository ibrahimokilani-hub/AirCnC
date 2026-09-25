using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HotelBooking.Core.Application.Features.Hotels.Discounts.Commands;

public sealed class AddDiscountCommandHandler(
    IAppDbContext context,
    IDistributedCache cache,
    TimeProvider timeProvider,
    IValidator<AddDiscountCommand> validator)
    : ICommandHandler<AddDiscountCommand, int>
{
    public async Task<Result<int>> HandleAsync(AddDiscountCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var hotel = await context.Hotels.FirstOrDefaultAsync(hotel => hotel.Id == command.HotelId, cancellationToken);
        if (hotel is null)
        {
            return Result<int>.Failure(HotelErrors.NotFound(command.HotelId));
        }

        var discount = hotel.AddDiscount(command.Name, Enum.Parse<DiscountType>(command.DiscountType, ignoreCase: true), command.Value, command.StartsAt, command.EndsAt);
        await context.SaveChangesAsync(cancellationToken);
        
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        await cache.RemoveAsync(CacheKeys.FeaturedDeals(today), cancellationToken);

        return Result<int>.Success(discount.Id);
    }
}

