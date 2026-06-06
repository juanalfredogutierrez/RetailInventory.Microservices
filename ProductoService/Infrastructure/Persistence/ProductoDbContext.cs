using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using ProductoService.Domain.Entities;

namespace ProductoService.Infrastructure.Persistence;

public class ProductoDbContext : DbContext
{
    public ProductoDbContext(DbContextOptions<ProductoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().ToTable("Producto");

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Precio).HasColumnType("decimal(18,2)");
        });
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}