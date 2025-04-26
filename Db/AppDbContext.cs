using Microsoft.EntityFrameworkCore;
using ProyectoCatedra.Models;

namespace ProyectoCatedra.Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<RecuperacionContraseña> RecuperacionContraseñas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<HistorialMovimiento> HistorialMovimientos { get; set; }

        // Nuevos DbSets
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
        .HasMany<DetalleVenta>()
        .WithOne(d => d.Producto)
        .HasForeignKey(d => d.ProductoId)
        .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<HistorialMovimiento>()
                .HasOne(h => h.Producto)
                .WithMany()
                .HasForeignKey(h => h.ProductoId);

            // Configuración de Venta y DetalleVenta
            modelBuilder.Entity<Venta>()
                .HasMany(v => v.Detalles)
                .WithOne(d => d.Venta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
