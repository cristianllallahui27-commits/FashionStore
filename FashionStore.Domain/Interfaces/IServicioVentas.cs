namespace FashionStore.Domain.Interfaces
{
    public interface IServicioVentas
    {
        Task<int> RegistrarVenta(int clienteId, int vendedorId, int metodoPagoId, List<DetalleVentaDTO> detalles, string? usuarioAutenticadoId = null);
        Task<bool> ActualizarInventario(int prendaId, int cantidad);
        Task<decimal> CalcularTotalVenta(List<DetalleVentaDTO> detalles);
        Task<(bool exito, string mensaje)> ValidarVenta(int clienteId, int vendedorId, int metodoPagoId, List<DetalleVentaDTO> detalles);
    }

    public class DetalleVentaDTO
    {
        public int PrendaId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
