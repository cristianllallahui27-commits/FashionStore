using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string DNI { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }

        public string Direccion { get; set; } = string.Empty;

        // Relación de navegación
        public ICollection<Venta>? Ventas { get; set; }
    }
}