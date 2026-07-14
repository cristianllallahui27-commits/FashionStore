namespace FashionStore.Domain.DTOs
{
    public class ConfiguracionAuditoriaDTO
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = null!;
        public string? NombreUsuario { get; set; }
        public string PropiedadModificada { get; set; } = null!;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public string? Descripcion { get; set; }
    }
}
