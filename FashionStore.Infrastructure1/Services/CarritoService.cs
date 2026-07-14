using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;

namespace FashionStore.Infrastructure.Services
{
    public class CarritoService : ICarritoService
    {
        private List<CarritoItem> _items = new();

        public void AgregarProducto(Prenda prenda, int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");

            if (prenda.Stock < cantidad)
                throw new InvalidOperationException("No hay suficiente stock disponible");

            var itemExistente = _items.FirstOrDefault(i => i.PrendaId == prenda.Id);

            if (itemExistente != null)
            {
                if (prenda.Stock < itemExistente.Cantidad + cantidad)
                    throw new InvalidOperationException("No hay suficiente stock disponible");

                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _items.Add(new CarritoItem
                {
                    PrendaId = prenda.Id,
                    NombrePrenda = prenda.Nombre,
                    Color = prenda.Color,
                    Talla = prenda.Talla,
                    Precio = prenda.Precio,
                    Cantidad = cantidad
                });
            }
        }

        public void ModificarCantidad(int prendaId, int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");

            var item = _items.FirstOrDefault(i => i.PrendaId == prendaId);
            if (item == null)
                throw new InvalidOperationException("Producto no encontrado en el carrito");

            item.Cantidad = nuevaCantidad;
        }

        public void EliminarProducto(int prendaId)
        {
            var item = _items.FirstOrDefault(i => i.PrendaId == prendaId);
            if (item != null)
                _items.Remove(item);
        }

        public void LimpiarCarrito()
        {
            _items.Clear();
        }

        public List<CarritoItem> ObtenerItems()
        {
            return _items.ToList();
        }

        public decimal CalcularSubtotal()
        {
            return _items.Sum(i => i.Subtotal);
        }

        public decimal CalcularTotal()
        {
            return CalcularSubtotal();
        }

        public int ObtenerCantidadTotal()
        {
            return _items.Sum(i => i.Cantidad);
        }
    }
}
