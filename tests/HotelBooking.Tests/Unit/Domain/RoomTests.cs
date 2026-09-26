using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class RoomTests
{
    [Fact]
    public void Create_TrimsTheNumber() =>
        Assert.Equal("101", Room.Create(1, 1, " 101 ").Number);

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Create_MissingParent_Throws(int hotelId, int roomTypeId) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Room.Create(hotelId, roomTypeId, "101"));

    [Fact]
    public void Update_BlankNumber_ThrowsAndLeavesTheRoomUnchanged()
    {
        var room = Room.Create(1, 1, "101");

        Assert.Throws<ArgumentException>(() => room.Update(2, " "));
        Assert.Equal("101", room.Number);
        Assert.Equal(1, room.RoomTypeId);
    }
}