namespace FashionStore.Domain.Entities
{
    /// <summary>
    /// Entidad para registrar cambios en la configuración del sistema
    /// </summary>
    public class ConfiguracionAuditoria
    {
        public int Id { get; set; }

        /// <summary>ID del usuario que realizó el cambio</summary>
        public string UsuarioId { get; set; } = null!;

        /// <summary>Nombre del usuario</summary>
        public string? NombreUsuario { get; set; }

        /// <summary>Propiedad que fue modificada</summary>
        public string PropiedadModificada { get; set; } = null!;

        /// <summary>Valor anterior</summary>
        public string? ValorAnterior { get; set; }

        /// <summary>Valor nuevo</summary>
        public string? ValorNuevo { get; set; }

        /// <summary>Fecha y hora del cambio</summary>
        public DateTime FechaCambio { get; set; } = DateTime.Now;

        /// <summary>Descripción del cambio (opcional)</summary>
        public string? Descripcion { get; set; }

        /// <summary>Navegación a Usuario (si existe en Identity)</summary>
        public virtual ApplicationUser? Usuario { get; set; }
    }
}
