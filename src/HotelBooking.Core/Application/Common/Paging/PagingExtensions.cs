using Microsoft.EntityFrameworkCore;
using HotelBooking.Contracts.Common;

namespace HotelBooking.Core.Application.Common.Paging;

public static class PagingExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
            CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<T>(items, page, pageSize, totalCount);
            
    }
}