namespace FashionStore.Domain.DTOs
{
    /// <summary>
    /// DTO para transferir datos de Configuración del Sistema
    /// </summary>
    public class ConfiguracionSistemaDTO
    {
        // ==========================================
        // IDENTIDAD Y BRANDING
        // ==========================================

        public string NombreTienda { get; set; } = "FashionStore";

        public string? RutaLogo { get; set; }

        public string? RutaFavicon { get; set; }

        public string? RutaImagenInstitucional { get; set; }

        // ==========================================
        // COLORES Y TEMA
        // ==========================================

        public string ColorPrimario { get; set; } = "#667eea";

        public string ColorSecundario { get; set; } = "#764ba2";

        public string ColorFondo { get; set; } = "#f5f7fa";

        public bool TemaOscuro { get; set; } = false;

        // ==========================================
        // FONDOS
        // ==========================================

        public string? RutaFondoLogin { get; set; }

        public string? RutaFondoDashboard { get; set; }

        // ==========================================
        // DATOS DEL NEGOCIO
        // ==========================================

        public string? NombrePropietario { get; set; }

        public string? Telefono { get; set; }

        public string? Correo { get; set; }

        public string? Direccion { get; set; }

        public string? Ciudad { get; set; }

        public string? Pais { get; set; }

        public string? CodigoPostal { get; set; }

        public string? RUC { get; set; }

        public string? Descripcion { get; set; }
    }
}
