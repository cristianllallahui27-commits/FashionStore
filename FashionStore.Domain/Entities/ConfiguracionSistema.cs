namespace FashionStore.Domain.Entities
{
    /// <summary>
    /// Entidad para almacenar la configuración global del sistema
    /// Solo debe haber un registro en la base de datos
    /// </summary>
    public class ConfiguracionSistema
    {
        public int Id { get; set; } = 1; // Siempre será 1 para garantizar un único registro

        // ==========================================
        // IDENTIDAD Y BRANDING
        // ==========================================

        /// <summary>Nombre de la tienda</summary>
        public string NombreTienda { get; set; } = "FashionStore";

        /// <summary>Logo</summary>
        public string? RutaLogo { get; set; }

        /// <summary>Favicon</summary>
        public string? RutaFavicon { get; set; }

        /// <summary>Imagen del Login</summary>
        public string? RutaImagenLogin { get; set; }

        /// <summary>Imagen institucional</summary>
        public string? RutaImagenInstitucional { get; set; }

        /// <summary>Banner principal</summary>
        public string? RutaBanner { get; set; }

        // ==========================================
        // FONDOS
        // ==========================================

        /// <summary>Fondo del Login</summary>
        public string? RutaFondoLogin { get; set; }

        /// <summary>Fondo del Dashboard</summary>
        public string? RutaFondoDashboard { get; set; }

        /// <summary>Fondo del Menú</summary>
        public string? RutaFondoMenu { get; set; }

        // ==========================================
        // COLORES Y TEMA
        // ==========================================

        /// <summary>Color primario</summary>
        public string ColorPrimario { get; set; } = "#667eea";

        /// <summary>Color secundario</summary>
        public string ColorSecundario { get; set; } = "#764ba2";

        /// <summary>Color del menú</summary>
        public string ColorMenu { get; set; } = "#2c3e50";

        /// <summary>Color de botones</summary>
        public string ColorBotones { get; set; } = "#667eea";

        /// <summary>Color del Dashboard</summary>
        public string ColorDashboard { get; set; } = "#ecf0f1";

        /// <summary>Color de fondo general</summary>
        public string ColorFondo { get; set; } = "#f5f7fa";

        /// <summary>Tema seleccionado</summary>
        public string TemaSeleccionado { get; set; } = "Fashion Store";

        /// <summary>Tema oscuro (true) o claro (false)</summary>
        public bool TemaOscuro { get; set; } = false;

        // ==========================================
        // DATOS DEL NEGOCIO
        // ==========================================

        /// <summary>Nombre del propietario</summary>
        public string? NombrePropietario { get; set; }

        /// <summary>Teléfono principal</summary>
        public string? Telefono { get; set; }

        /// <summary>Correo principal</summary>
        public string? Correo { get; set; }

        /// <summary>Dirección del negocio</summary>
        public string? Direccion { get; set; }

        /// <summary>Ciudad</summary>
        public string? Ciudad { get; set; }

        /// <summary>País</summary>
        public string? Pais { get; set; }

        /// <summary>Código postal</summary>
        public string? CodigoPostal { get; set; }

        /// <summary>RUC o número de identificación del negocio</summary>
        public string? RUC { get; set; }

        /// <summary>Descripción o misión del negocio</summary>
        public string? Descripcion { get; set; }

        // ==========================================
        // REDES SOCIALES
        // ==========================================

        /// <summary>URL de Facebook</summary>
        public string? FacebookUrl { get; set; }

        /// <summary>URL de Instagram</summary>
        public string? InstagramUrl { get; set; }

        /// <summary>URL de Twitter</summary>
        public string? TwitterUrl { get; set; }

        /// <summary>URL de LinkedIn</summary>
        public string? LinkedInUrl { get; set; }

        /// <summary>URL de TikTok</summary>
        public string? TikTokUrl { get; set; }

        // ==========================================
        // PIE DE PÁGINA
        // ==========================================

        /// <summary>Texto del pie de página</summary>
        public string? TextoPiePagina { get; set; } = "&copy; 2025 FashionStore. Todos los derechos reservados.";

        // ==========================================
        // AUDITORÍA
        // ==========================================

        /// <summary>Fecha de creación</summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>Fecha de última actualización</summary>
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
