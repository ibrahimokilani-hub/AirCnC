using FluentValidation;
using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Users.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Users.Queries;

public sealed class GetUsersQueryHandler(IAppDbContext context, IValidator<GetUsersQuery> validator)
    : IQueryHandler<GetUsersQuery, PagedResult<UserListItem>>
{
    public async Task<Result<PagedResult<UserListItem>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PagedResult<UserListItem>>.Failure(validation.ToValidationError());
        }

        var page = await context.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .Select(user => new UserListItem(
                user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(), user.CreatedAtUtc))
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result<PagedResult<UserListItem>>.Success(page);
    }
}