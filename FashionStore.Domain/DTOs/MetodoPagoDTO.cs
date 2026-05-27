using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.DTOs
{
    public class MetodoPagoDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}