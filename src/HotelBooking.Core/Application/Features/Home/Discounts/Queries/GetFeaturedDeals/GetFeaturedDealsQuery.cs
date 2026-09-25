using HotelBooking.Contracts.Home;
using HotelBooking.Contracts.Home.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Home.Discounts.Queries.GetFeaturedDeals;

public sealed record GetFeaturedDealsQuery : IQuery<IReadOnlyList<FeaturedDealResponse>>
{
    public const int Count = 5;
}