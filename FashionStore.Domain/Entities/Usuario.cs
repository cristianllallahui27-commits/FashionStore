using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities;

/// <summary>
/// Representa un usuario del sistema
/// </summary>
public class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

 [Required(ErrorMessage = "El nombre de usuario es requerido")]
 [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 100 caracteres")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
  [StringLength(150)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(255)]
    public string Contraseña { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string? Apellido { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Required]
    public bool Activo { get; set; } = true;

    // Relaciones
    [Required]
    public int RolId { get; set; }
    public virtual Rol? Rol { get; set; }

    // Un usuario puede ser vendedor (opcional)
    public int? VendedorId { get; set; }
  public virtual Vendedor? Vendedor { get; set; }

    // Un usuario puede ser cliente (opcional)
    public int? ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
}
