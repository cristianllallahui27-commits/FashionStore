using FashionStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Infrastructure.Context
{
    public class FashionStoreDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public FashionStoreDbContext(
            DbContextOptions<FashionStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Prenda> Prendas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Vendedor> Vendedores { get; set; }

        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        public DbSet<DescuentoAutorizado> DescuentosAutorizados { get; set; }

        // public DbSet<AlertaStock> AlertasStock { get; set; } // Pendiente: agregar en futuras migraciones

        public DbSet<ConfiguracionSistema> Configuraciones { get; set; }

        public DbSet<ConfiguracionAuditoria> ConfiguracionesAuditoria { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // MAPEO EXPLÍCITO DE NOMBRES DE TABLA
            // Las migraciones renombraron las tablas a singular.
            // ==========================================
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<MetodoPago>().ToTable("MetodoPago");
            modelBuilder.Entity<Venta>().ToTable("Venta");
            modelBuilder.Entity<DetalleVenta>().ToTable("DetalleVenta");
            modelBuilder.Entity<Vendedor>().ToTable("Vendedor");

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Prenda>()
                .Property(p => p.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Prenda>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            // ==========================================
            // CONFIGURACIÓN DEL SISTEMA
            // ==========================================

            modelBuilder.Entity<ConfiguracionSistema>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.RutaLogo)
                .HasColumnName("LogoUrl");

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.RutaFavicon)
                .HasColumnName("FaviconUrl");

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.NombreTienda)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorPrimario)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorSecundario)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorMenu)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorBotones)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorDashboard)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.ColorFondo)
                .HasMaxLength(7)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.TemaSeleccionado)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.Telefono)
                .HasMaxLength(20);

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.Correo)
                .HasMaxLength(100);

            modelBuilder.Entity<ConfiguracionSistema>()
                .Property(c => c.RUC)
                .HasMaxLength(20);

            // ==========================================
            // AUDITORÍA DE CONFIGURACIÓN
            // ==========================================

            modelBuilder.Entity<ConfiguracionAuditoria>()
                .HasKey(ca => ca.Id);

            modelBuilder.Entity<ConfiguracionAuditoria>()
                .Property(ca => ca.PropiedadModificada)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ConfiguracionAuditoria>()
                .Property(ca => ca.NombreUsuario)
                .HasMaxLength(100);

            modelBuilder.Entity<ConfiguracionAuditoria>()
                .HasOne(ca => ca.Usuario)
                .WithMany()
                .HasForeignKey(ca => ca.UsuarioId)
                .IsRequired(false);

            modelBuilder.Entity<ConfiguracionAuditoria>()
                .HasIndex(ca => ca.FechaCambio)
                .IsDescending();

            // ==========================================
            // ALERTAS DE STOCK (Pendiente para futuras migraciones)
            // ==========================================
            // Configuración de AlertaStock será agregada cuando se implemente
        }
    }
}