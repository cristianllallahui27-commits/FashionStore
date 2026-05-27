using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities
{
    public class MetodoPago
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Venta>? Ventas { get; set; }
    }
}