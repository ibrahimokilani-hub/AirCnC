using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Queries.GetNearbyAttractions;

/// <summary>The standalone attraction catalog grid: paged, searchable, sortable.</summary>
public sealed class GetNearbyAttractionsQueryHandler(IAppDbContext context, IValidator<GetNearbyAttractionsQuery> validator)
    : IQueryHandler<GetNearbyAttractionsQuery, PagedResult<NearbyAttractionResponse>>
{
    public async Task<Result<PagedResult<NearbyAttractionResponse>>> HandleAsync(
        GetNearbyAttractionsQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<NearbyAttractionResponse>>.Failure(validation.ToValidationError());
        }

        var attractions = context.NearbyAttractions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            attractions = attractions.Where(a => a.Name.Contains(term) || a.Category.Contains(term));
        }

        var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        // Stable secondary sort by Id so paging is deterministic when the sort key ties.
        IOrderedQueryable<NearbyAttraction> ordered = query.SortBy?.ToLowerInvariant() switch
        {
            "category" => descending
                ? attractions.OrderByDescending(a => a.Category)
                : attractions.OrderBy(a => a.Category),
            _ => descending
                ? attractions.OrderByDescending(a => a.Name)
                : attractions.OrderBy(a => a.Name)
        };

        var page = await ordered
            .ThenBy(a => a.Id)
            .Select(a => new NearbyAttractionResponse(a.Id, a.Name, a.Category, a.Latitude, a.Longitude, a.AttractionImageUrl))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<NearbyAttractionResponse>>.Success(page);
    }
}
