using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HotelBooking.Core.Application.Features.Hotels.Discounts.Commands.RemoveDiscount;


public sealed class RemoveDiscountCommandHandler(IAppDbContext context, IDistributedCache cache, TimeProvider timeProvider)
    : ICommandHandler<RemoveDiscountCommand>
{
    public async Task<Result> HandleAsync(RemoveDiscountCommand command, CancellationToken cancellationToken)
    {
        var discount = await context.Discounts.FirstOrDefaultAsync(
            discount => discount.Id == command.DiscountId && discount.HotelId == command.HotelId,
            cancellationToken);

        if (discount is null)
        {
            return Result.Failure(Error.NotFound(
                "Discount.NotFound",
                $"Hotel '{command.HotelId}' has no discount with ID '{command.DiscountId}'."));
        }

        context.Discounts.Remove(discount);
        await context.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        await cache.RemoveAsync(CacheKeys.FeaturedDeals(today), cancellationToken);

        return Result.Success();
    }
}