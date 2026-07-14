using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Domain.Entities
{
    public class Prenda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Talla { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        [Range(1, 99999)]
        public decimal Precio { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        public string? ImagenUrl { get; set; }

        /// <summary>Código de barras opcional — listo para integración futura con lector</summary>
        [StringLength(50)]
        public string? CodigoBarra { get; set; }

        // Clave forénea
        public int CategoriaId { get; set; }

        // NOTA: Los siguientes campos estarán disponibles en migraciones futuras cuando se requiera:
        // [Range(0, 10000)]
        // [Required]
        // public int StockMinimo { get; set; } = 5;
        // [Required]
        // public bool Estado { get; set; } = true;

        // Propiedades calculadas (no mapeadas a BD)
        [NotMapped]
        public bool Disponibilidad => Stock > 0;

        [NotMapped]
        public bool EstaAgotandose => Stock > 0 && Stock <= 5;

        // Navegación
        public Categoria? Categoria { get; set; }

        public ICollection<DetalleVenta>? DetalleVentas { get; set; }
    }
}
