using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Domain.Entities
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        // Venta
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        // Prenda
        public int PrendaId { get; set; }
        public Prenda? Prenda { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
    }
}