using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Abstractions;

public interface IIdentityService
{
    Task<Result<int>> RegisterAsync(NewUser user, CancellationToken cancellationToken);
}

public sealed record NewUser(string Email, string Password, string FirstName, string LastName);