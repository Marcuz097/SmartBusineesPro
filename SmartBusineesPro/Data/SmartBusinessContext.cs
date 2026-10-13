using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SmartBusineesPro.Models;

namespace SmartBusineesPro.Data
{
    public class SmartBusinessContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;

        public SmartBusinessContext()
        {
        }

        public SmartBusinessContext(DbContextOptions<SmartBusinessContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Lee la cadena definida en App.config
                string? connectionString = ConfigurationManager.ConnectionStrings["SmartBusinessPro"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena 'SmartBusinessPro' en App.config.");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la relación 1 a N entre Categoria y Producto
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");
        }
    }
}
