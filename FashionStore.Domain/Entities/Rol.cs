using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities;

/// <summary>
/// Representa un rol del sistema
/// </summary>
public class Rol
{
    [Key]
    public int RolId { get; set; }

    [Required(ErrorMessage = "El nombre del rol es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    public virtual ICollection<Usuario> Usuarios { get; set; } = [];
}
