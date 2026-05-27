using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
    public class CategoriaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }
    }
}