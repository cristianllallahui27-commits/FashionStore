using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCompleto { get; set; } = string.Empty;

        // Alias para compatibilidad con vistas que usan .Nombre
        public string Nombre => NombreCompleto;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres")]
        public string? Email { get; set; }

        public string Telefono { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;
    }
}