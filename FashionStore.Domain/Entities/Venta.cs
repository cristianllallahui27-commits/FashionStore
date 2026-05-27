using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Cliente
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Vendedor
        public int VendedorId { get; set; }
        public Vendedor? Vendedor { get; set; }

        // Método Pago
        public int MetodoPagoId { get; set; }
        public MetodoPago? MetodoPago { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public ICollection<DetalleVenta>? DetalleVentas { get; set; }
    }
}