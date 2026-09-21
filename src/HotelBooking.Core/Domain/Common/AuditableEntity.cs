namespace HotelBooking.Core.Domain.Common;

public abstract class AuditableEntity : Entity, IAuditable, ISoftDeletable
{
    public DateTime CreatedAtUtc { get; private set; }

    public int? CreatedBy { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public int? UpdatedBy { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }
}