using FashionStore.Domain.Entities;

namespace FashionStore.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
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

        IGenericRepository<ConfiguracionSistema> Configuraciones { get; }

        IGenericRepository<ConfiguracionAuditoria> ConfiguracionesAuditoria { get; }

        IGenericRepository<DescuentoAutorizado> DescuentosAutorizados { get; }

        // ============================
        // GUARDAR
        // ============================

        Task<int> CommitAsync();
    }
}