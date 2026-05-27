using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
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

        public string? ImagenUrl { get; set; }
    }
}