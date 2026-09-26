using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Infrastructure.Security;

namespace HotelBooking.Tests.Unit.Security;

public sealed class ClaimsCreatorTests
{
    private static readonly AuthUser Admin = new(7, "sara@example.com", "Sara", "Haddad", UserRole.Admin);

    [Fact]
    public void GenerateClaims_CarriesTheIdEmailNameAndRole()
    {
        var claims = ClaimsCreator.GenerateClaims(Admin);

        Assert.Equal("7", claims.Single(claim => claim.Type == "sub").Value);
        Assert.Equal("sara@example.com", claims.Single(claim => claim.Type == "email").Value);
        Assert.Equal("Sara Haddad", claims.Single(claim => claim.Type == "name").Value);
        Assert.Equal(nameof(UserRole.Admin), claims.Single(claim => claim.Type == AuthClaims.Role).Value);
    }

    /// <summary>The jti is per token, so two tokens for one user are still distinguishable.</summary>
    [Fact]
    public void GenerateClaims_GivesEachTokenItsOwnJti()
    {
        var first = ClaimsCreator.GenerateClaims(Admin).Single(claim => claim.Type == "jti").Value;
        var second = ClaimsCreator.GenerateClaims(Admin).Single(claim => claim.Type == "jti").Value;

        Assert.NotEqual(first, second);
    }
}