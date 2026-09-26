using HotelBooking.Infrastructure.Security;

namespace HotelBooking.Tests.Unit.Security;

public sealed class RefreshTokenTests
{
    private static readonly DateTime NowUtc = new(2026, 6, 10, 9, 0, 0, DateTimeKind.Utc);

    private static RefreshToken Token() => RefreshToken.Create(1, "hash", NowUtc, NowUtc.AddDays(7));

    [Theory]
    [InlineData(0, true)]
    [InlineData(6, true)]
    [InlineData(7, false)]
    [InlineData(8, false)]
    public void IsActive_LastsUntilExpiry(int daysLater, bool expected) =>
        Assert.Equal(expected, Token().IsActive(NowUtc.AddDays(daysLater)));

    [Fact]
    public void Revoke_MakesItInactiveEvenBeforeExpiry()
    {
        var token = Token();

        token.Revoke(NowUtc.AddDays(1));

        Assert.False(token.IsActive(NowUtc.AddDays(2)));
        Assert.Equal(NowUtc.AddDays(1), token.RevokedAtUtc);
    }
}