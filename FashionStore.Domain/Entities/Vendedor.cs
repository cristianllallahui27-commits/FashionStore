using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities
{
    public class Vendedor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Telefono { get; set; }

        [StringLength(150)]
        public string? Correo { get; set; }

        public bool Estado { get; set; } = true;

        public ICollection<Venta>? Ventas { get; set; }
    }
}