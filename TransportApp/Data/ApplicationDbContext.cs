using Microsoft.EntityFrameworkCore;
using TransportApp.Models;

namespace TransportApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Viaje> Viajes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Viaje>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Destino).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Estado).HasConversion<int>();
            });
        }
    }
}