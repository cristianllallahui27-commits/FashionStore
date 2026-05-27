using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8)]
        public string DNI { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }
    }
}