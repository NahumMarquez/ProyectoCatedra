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
    }
}