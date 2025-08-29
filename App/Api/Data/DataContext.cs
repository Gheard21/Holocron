using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Holocron.App.Api.Data;

public class DataContext(DbContextOptions<DataContext> options, IIdentityProvider identityProvider) : DbContext(options)
{
    public DbSet<LikeEntity> Likes { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LikeEntity>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(255);
            b.HasIndex(e => e.Name).IsUnique();
            b.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<CommentEntity>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(255);
            b.Property(e => e.DateWatched)
                .IsRequired();
            b.Property(e => e.Rating)
                .IsRequired();
            b.Property(e => e.Review)
                .IsRequired()
                .HasMaxLength(2000);

            b.HasIndex(e => e.TenantId);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        SetTenantId();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTenantId();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetTenantId()
    {
        var tenantId = identityProvider.GetCurrentUserId();

        if (string.IsNullOrEmpty(tenantId)) return;

        foreach (var entry in ChangeTracker.Entries<LikeEntity>()
                     .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = tenantId;
        }

        foreach (var entry in ChangeTracker.Entries<CommentEntity>()
                     .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = tenantId;
        }
    }
}
