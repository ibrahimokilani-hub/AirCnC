using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class HotelImage : Entity
{
    public const int ImageUrlMaxLength = 2048;

    private HotelImage(int hotelId, string imageUrl)
    {
        HotelId = hotelId;
        ImageUrl = imageUrl.Trim();
    }

    public int HotelId { get; private set; }

    public string ImageUrl { get; private set; }

    public static HotelImage Create(int hotelId, string imageUrl)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hotelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(imageUrl);

        return new HotelImage(hotelId, imageUrl.Trim());
    }

    public void Update(string imageUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imageUrl);

        ImageUrl = imageUrl.Trim();
    }
}
