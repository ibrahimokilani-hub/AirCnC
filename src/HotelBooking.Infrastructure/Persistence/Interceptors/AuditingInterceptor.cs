using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelBooking.Infrastructure.Persistence.Interceptors;

public sealed class AuditingInterceptor(ICurrentUser currentUser, TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null) return;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userId = currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // 1. Handle Soft Delete
            if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable })
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                entry.Property(nameof(ISoftDeletable.DeletedAtUtc)).CurrentValue = now;
            }

            // 2. Handle Auditing based on the final state
            if (entry.Entity is IAuditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(nameof(IAuditable.CreatedAtUtc)).CurrentValue = now;
                        entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = userId;
                        break;
                        
                    case EntityState.Modified:
                        entry.Property(nameof(IAuditable.UpdatedAtUtc)).CurrentValue = now;
                        entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = userId;
                        break;
                }
            }
        }
    }
}