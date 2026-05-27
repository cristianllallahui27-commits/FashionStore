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

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Prenda>()
                .Property(p => p.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Prenda>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);
        }
    }
}