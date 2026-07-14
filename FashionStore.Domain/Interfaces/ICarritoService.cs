using FashionStore.Domain.Entities;

namespace FashionStore.Domain.Interfaces
{
    public interface ICarritoService
    {
        void AgregarProducto(Prenda prenda, int cantidad);
        void ModificarCantidad(int prendaId, int nuevaCantidad);
        void EliminarProducto(int prendaId);
        void LimpiarCarrito();
        List<CarritoItem> ObtenerItems();
        decimal CalcularSubtotal();
        decimal CalcularTotal();
        int ObtenerCantidadTotal();
    }

    public class CarritoItem
    {
        public int PrendaId { get; set; }
        public string NombrePrenda { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Talla { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }
}
