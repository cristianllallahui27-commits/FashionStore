using FashionStore.Domain.Entities;

namespace FashionStore.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        // ============================
        // TABLAS
        // ============================

        IGenericRepository<Categoria> Categorias { get; }

        IGenericRepository<Prenda> Prendas { get; }

        IGenericRepository<Cliente> Clientes { get; }

        IGenericRepository<Vendedor> Vendedores { get; }

        IGenericRepository<Venta> Ventas { get; }

        IGenericRepository<DetalleVenta> DetalleVentas { get; }

        IGenericRepository<MetodoPago> MetodosPago { get; }

        // ============================
        // GUARDAR
        // ============================

        Task<int> CommitAsync();
    }
}