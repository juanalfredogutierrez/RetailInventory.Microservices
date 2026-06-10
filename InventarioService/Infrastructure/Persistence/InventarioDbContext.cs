using BuildingBlocks.Domain;
using InventarioService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Infrastructure.Persistence;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
        : base(options) { }

    public DbSet<MovimientoInventario> Movimientos => Set<MovimientoInventario>();
    public DbSet<DetalleMovimientoInventario> Detalles => Set<DetalleMovimientoInventario>();
    public DbSet<ExistenciaProducto> Existencias => Set<ExistenciaProducto>();
    public DbSet<EventoProcesado> EventosProcesados => Set<EventoProcesado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovimientoInventario>().ToTable("MovimientoInventario");
        modelBuilder.Entity<DetalleMovimientoInventario>().ToTable("DetalleMovimientoInventario");
        modelBuilder.Entity<ExistenciaProducto>().ToTable("ExistenciaProducto");
        modelBuilder.Entity<EventoProcesado>().ToTable("EventoProcesado");
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
                entry.Entity.UpdatedAt = DateTime.Now;
            }

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}