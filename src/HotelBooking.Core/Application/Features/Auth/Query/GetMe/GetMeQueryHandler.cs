using HotelBooking.Contracts.Auth.Responses;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Auth.Query.GetMe;

public sealed class GetMeQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    : IQueryHandler<GetMeQuery, LoggedInUserResponse>
{
    public async Task<Result<LoggedInUserResponse>> HandleAsync(
        GetMeQuery query,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result<LoggedInUserResponse>.Failure(AuthErrors.NotAuthenticated);
        }

        var user = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new LoggedInUserResponse(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString()))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result<LoggedInUserResponse>.Failure(AuthErrors.NotAuthenticated)
            : Result<LoggedInUserResponse>.Success(user);
    }
}