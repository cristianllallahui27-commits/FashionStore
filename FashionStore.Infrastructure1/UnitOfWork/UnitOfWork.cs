using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.Repositories;

namespace FashionStore.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FashionStoreDbContext _context;

        public UnitOfWork(FashionStoreDbContext context)
        {
            _context = context;

            Categorias =
                new GenericRepository<Categoria>(_context);

            Prendas =
                new GenericRepository<Prenda>(_context);

            Clientes =
                new GenericRepository<Cliente>(_context);

            Vendedores =
                new GenericRepository<Vendedor>(_context);

            Ventas =
                new GenericRepository<Venta>(_context);

            DetalleVentas =
                new GenericRepository<DetalleVenta>(_context);

            MetodosPago =
                new GenericRepository<MetodoPago>(_context);
        }

        // ====================================
        // REPOSITORIOS
        // ====================================

        public IGenericRepository<Categoria> Categorias { get; }

        public IGenericRepository<Prenda> Prendas { get; }

        public IGenericRepository<Cliente> Clientes { get; }

        public IGenericRepository<Vendedor> Vendedores { get; }

        public IGenericRepository<Venta> Ventas { get; }

        public IGenericRepository<DetalleVenta> DetalleVentas { get; }

        public IGenericRepository<MetodoPago> MetodosPago { get; }

        // ====================================
        // GUARDAR
        // ====================================

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}