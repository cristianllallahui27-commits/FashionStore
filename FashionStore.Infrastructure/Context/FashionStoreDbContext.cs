using FashionStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Infrastructure.Context
{
    public class FashionStoreDbContext : IdentityDbContext<ApplicationUser>
    {
        public FashionStoreDbContext(DbContextOptions<FashionStoreDbContext> options)
            : base(options)
        {
        }

        // TABLAS

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Prenda> Prendas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Vendedor> Vendedores { get; set; }

        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        public DbSet<DescuentoAutorizado> DescuentosAutorizados { get; set; }

        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }

        // FLUENT API

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Categoria

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .HasMaxLength(100);

            // Prenda

            modelBuilder.Entity<Prenda>()
                .Property(p => p.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Prenda>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Prendas)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cliente

            modelBuilder.Entity<Cliente>()
                .Property(c => c.NombreCompleto)
                .HasMaxLength(150);

            // Vendedor

            modelBuilder.Entity<Vendedor>()
                .Property(v => v.Nombres)
                .HasMaxLength(150);

            // MetodoPago

            modelBuilder.Entity<MetodoPago>()
                .Property(m => m.Nombre)
                .HasMaxLength(50);

            // Venta

            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Vendedor)
                .WithMany(vd => vd.Ventas)
                .HasForeignKey(v => v.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.MetodoPago)
                .WithMany(mp => mp.Ventas)
                .HasForeignKey(v => v.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            // DetalleVenta

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.DetalleVentas)
                .HasForeignKey(d => d.VentaId);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Prenda)
                .WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.PrendaId);
        }
    }
}