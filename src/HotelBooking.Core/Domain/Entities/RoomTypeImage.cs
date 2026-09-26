using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class RoomTypeImage : Entity
{
    public const int ImageUrlMaxLength = 2048;

    private RoomTypeImage(int roomTypeId, string imageUrl)
    {
        RoomTypeId = roomTypeId;
        ImageUrl = imageUrl.Trim();
    }

    public int RoomTypeId { get; private set; }
    
    public string ImageUrl { get; private set; }

    public static RoomTypeImage Create(int roomTypeId, string imageUrl)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        
        return new RoomTypeImage(roomTypeId, imageUrl.Trim());
    }

    public void Update(string imageUrl)
    {
        ImageUrl = imageUrl.Trim();
    }
    
}
