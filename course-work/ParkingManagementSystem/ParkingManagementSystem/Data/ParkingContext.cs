using Microsoft.EntityFrameworkCore;
using ParkingManager.Models;

namespace ParkingManager.Data
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options)
        {
        }

        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingSession> ParkingSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация на ограничението за дължина на текстовите полета
            modelBuilder.Entity<ParkingLot>()
                .Property(p => p.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<ParkingLot>()
                .Property(p => p.Address).HasMaxLength(200).IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.PlateNumber).HasMaxLength(10).IsRequired();
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.OwnerName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Model).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Type).HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Color).HasMaxLength(20).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
