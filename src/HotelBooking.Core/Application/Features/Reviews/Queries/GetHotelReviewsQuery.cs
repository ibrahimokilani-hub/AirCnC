using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Reviews.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Reviews.Queries;

public sealed record GetHotelReviewsQuery(int HotelId, int Page, int PageSize) : IQuery<PagedResult<ReviewResponse>>;
