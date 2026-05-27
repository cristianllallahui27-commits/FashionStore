using System.ComponentModel.DataAnnotations;

namespace FashionStore.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; }

        public string DNI { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }
    }
}