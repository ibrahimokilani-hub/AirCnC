using System.Linq.Expressions;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<City> Cities => Set<City>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteFilters(modelBuilder);

    }
    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        var softDeletableTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in softDeletableTypes)
        {
            // Builds: (TEntity entity) => !entity.IsDeleted
            var entity = Expression.Parameter(entityType.ClrType, "entity");
            var isDeleted = Expression.Property(entity, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeleted), entity);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}