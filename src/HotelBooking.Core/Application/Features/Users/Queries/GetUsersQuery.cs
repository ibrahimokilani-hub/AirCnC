using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Users.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;

namespace HotelBooking.Core.Application.Features.Users.Queries;

public sealed record GetUsersQuery(int Page, int PageSize) : IQuery<PagedResult<UserListItem>>;
