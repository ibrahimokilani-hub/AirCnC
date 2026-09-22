using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Features.Auth;
using HotelBooking.Core.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> userManager): IIdentityService
{
    public async Task<Result<int>> RegisterAsync(NewUser newUser, CancellationToken cancellationToken)
    {
        var userTemp = await userManager.FindByEmailAsync(newUser.Email);

        if (userTemp is not null)
        {
            return Result<int>.Failure(AuthErrors.EmailTaken(newUser.Email));
        }

        var user = new ApplicationUser()
        {
            UserName = $"{newUser.FirstName[0]}{newUser.LastName}",
            Email = newUser.Email,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName
        };

        var createdUser = await userManager.CreateAsync(user);

        if (!createdUser.Succeeded)
        {
            return Result<int>.Failure(new Error(ErrorType.Conflict, createdUser.Errors.First().Code, createdUser.Errors.First().Description));
        }

        return Result<int>.Success(user.Id);
    }
}