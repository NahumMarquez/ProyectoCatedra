﻿using Microsoft.EntityFrameworkCore;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)"); // Definir precisión y escala

            base.OnModelCreating(modelBuilder);
        }

    }
}