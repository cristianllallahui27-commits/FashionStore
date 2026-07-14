using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        /// <summary>ID del usuario de Identity asociado a este vendedor (para cambio de contraseña) - NO MAPEADO A BD</summary>
        [NotMapped]
        public string? UsuarioId { get; set; }

        public bool Estado { get; set; } = true;

        /// <summary>Última contraseña asignada por el administrador (visible solo para Admin)</summary>
        [StringLength(100)]
        public string? UltimaPasswordAdmin { get; set; }

        public ICollection<Venta>? Ventas { get; set; }
    }
}