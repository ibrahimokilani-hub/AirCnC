using HotelBooking.Contracts.Common;

namespace HotelBooking.Tests.Unit.Common;

public sealed class PagedResultTests
{
    [Theory]
    [InlineData(0, 25, 0)]
    [InlineData(1, 25, 1)]
    [InlineData(25, 25, 1)]
    [InlineData(26, 25, 2)]
    [InlineData(51, 25, 3)]
    public void TotalPages_RoundsUp(int totalCount, int pageSize, int expected) =>
        Assert.Equal(expected, new PagedResult<string>([], 1, pageSize, totalCount).TotalPages);
}