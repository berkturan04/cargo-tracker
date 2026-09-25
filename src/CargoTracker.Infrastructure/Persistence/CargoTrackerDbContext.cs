using CargoTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTracker.Infrastructure.Persistence;

public class CargoTrackerDbContext : DbContext
{
    public CargoTrackerDbContext(DbContextOptions<CargoTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(s => s.TrackingNumber)
                .IsUnique();

            entity.Property(s => s.ReceiverName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(s => s.OriginCity)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.DestinationCity)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.WeightKg)
                .HasPrecision(10, 2);

            entity.HasMany(s => s.StatusHistory)
                .WithOne()
                .HasForeignKey(sh => sh.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShipmentStatusHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();
        });
    }
}