namespace HotelBooking.Core.Domain.Common;

public interface IAuditable
{
    DateTime CreatedAtUtc { get; }
    int? CreatedBy { get; }
    DateTime? UpdatedAtUtc { get; }
    int? UpdatedBy { get; }
}
