using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
    // Clase auxiliar para que las vistas puedan usar .Categoria?.Nombre
    public class CategoriaInfo
    {
        public string? Nombre { get; set; }
    }

    public class PrendaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string? Nombre { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }

        [Required]
        public string? Talla { get; set; }

        [Required]
        public string? Color { get; set; }

        [Range(1, 99999)]
        public decimal Precio { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        public string? Imagen { get; set; }

        public int CategoriaId { get; set; }

        public string? CategoriaNombre { get; set; }

        // Propiedad de navegación para compatibilidad con vistas que usan .Categoria?.Nombre
        public CategoriaInfo? Categoria => CategoriaNombre != null
            ? new CategoriaInfo { Nombre = CategoriaNombre }
            : null;

        public string? ImagenUrl { get; set; }

        /// <summary>Código de barras opcional — listo para integración futura con lector</summary>
        [StringLength(50)]
        public string? CodigoBarra { get; set; }
    }
}