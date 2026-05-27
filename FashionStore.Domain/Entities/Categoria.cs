using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        // Relación
        public ICollection<Prenda>? Prendas { get; set; }
    }
}