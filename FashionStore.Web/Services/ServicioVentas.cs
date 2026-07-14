using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;

namespace FashionStore.Web.Services
{
    public interface IServicioVentasWeb
    {
        Task<Venta?> CrearVenta(int clienteId, int vendedorId, int metodoPagoId, decimal total);
        Task<Venta?> ObtenerVenta(int ventaId);
        Task<List<Venta>> ObtenerVentas();
    }

    public class ServicioVentas : IServicioVentasWeb
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServicioVentas(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Venta?> CrearVenta(int clienteId, int vendedorId, int metodoPagoId, decimal total)
        {
            return await Task.FromResult<Venta?>(null);
        }

        public async Task<Venta?> ObtenerVenta(int ventaId)
        {
            return await Task.FromResult<Venta?>(null);
        }

        public async Task<List<Venta>> ObtenerVentas()
        {
            return await Task.FromResult(new List<Venta>());
        }
    }
}
