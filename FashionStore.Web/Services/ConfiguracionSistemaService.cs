using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FashionStore.Web.Services
{
    /// <summary>
    /// Servicio para gestionar la configuración del sistema
    /// </summary>
    public interface IConfiguracionSistemaService
    {
        Task<ConfiguracionSistema> ObtenerConfiguracionAsync();
        Task ActualizarConfiguracionAsync(ConfiguracionSistema configuracion, string usuarioId, string nombreUsuario);
        Task GuardarImagenAsync(string nombreCampo, IFormFile archivo);
        Task<ConfiguracionSistema> RestablecerConfiguracionPorDefectoAsync(string usuarioId, string nombreUsuario);
        void LimpiarCache();
    }

    public class ConfiguracionSistemaService : IConfiguracionSistemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ConfiguracionSistemaService> _logger;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "ConfiguracionSistema_Id_1";
        private const int CACHE_DURATION_MINUTES = 60;

        public ConfiguracionSistemaService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment environment,
            ILogger<ConfiguracionSistemaService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Obtiene la configuración del sistema (siempre el mismo registro con Id=1)
        /// Utiliza caché en memoria para mejorar el rendimiento
        /// </summary>
        public async Task<ConfiguracionSistema> ObtenerConfiguracionAsync()
        {
            try
            {
                // Intentar obtener de caché
                if (_cache.TryGetValue(CACHE_KEY, out ConfiguracionSistema? cachedConfig))
                {
                    _logger.LogInformation("Configuración obtenida de caché");
                    return cachedConfig!;
                }

                var configuracion = await _unitOfWork.Configuraciones.GetByIdAsync(1);

                if (configuracion == null)
                {
                    // Si no existe, crear la configuración por defecto
                    configuracion = new ConfiguracionSistema
                    {
                        Id = 1,
                        NombreTienda = "FashionStore",
                        ColorPrimario = "#667eea",
                        ColorSecundario = "#764ba2",
                        ColorMenu = "#2c3e50",
                        ColorBotones = "#667eea",
                        ColorDashboard = "#ecf0f1",
                        ColorFondo = "#f5f7fa",
                        TemaSeleccionado = "Fashion Store",
                        TemaOscuro = false,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now
                    };

                    await _unitOfWork.Configuraciones.AddAsync(configuracion);
                    await _unitOfWork.CommitAsync();
                }

                // Guardar en caché
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES)
                };
                _cache.Set(CACHE_KEY, configuracion, cacheOptions);

                return configuracion;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener configuración: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza la configuración del sistema con auditoría
        /// </summary>
        public async Task ActualizarConfiguracionAsync(ConfiguracionSistema configuracion, string usuarioId, string nombreUsuario)
        {
            try
            {
                configuracion.FechaActualizacion = DateTime.Now;

                var existente = await _unitOfWork.Configuraciones.GetByIdAsync(1);
                var esNuevoRegistro = existente == null;

                if (esNuevoRegistro)
                {
                    existente = new ConfiguracionSistema
                    {
                        Id = 1,
                        FechaCreacion = DateTime.Now
                    };
                }

                // Registrar cambios en auditoría
                var cambios = new List<ConfiguracionAuditoria>();

                // Null check: si no existe configuración previa, crear default
                if (existente == null)
                {
                    existente = new ConfiguracionSistema
                    {
                        NombreTienda = configuracion.NombreTienda ?? "FashionStore",
                        FechaActualizacion = DateTime.Now
                    };
                }

                // Verificar y registrar cada cambio
                RegistrarCambioAuditoria(cambios, existente, configuracion, usuarioId, nombreUsuario);

                // Actualizar solo los campos que vinieron del formulario
                existente.NombreTienda = configuracion.NombreTienda;
                existente.ColorPrimario = configuracion.ColorPrimario;
                existente.ColorSecundario = configuracion.ColorSecundario;
                existente.ColorMenu = configuracion.ColorMenu;
                existente.ColorBotones = configuracion.ColorBotones;
                existente.ColorDashboard = configuracion.ColorDashboard;
                existente.ColorFondo = configuracion.ColorFondo;
                existente.TemaSeleccionado = configuracion.TemaSeleccionado;
                existente.TemaOscuro = configuracion.TemaOscuro;
                existente.NombrePropietario = configuracion.NombrePropietario;
                existente.Telefono = configuracion.Telefono;
                existente.Correo = configuracion.Correo;
                existente.Direccion = configuracion.Direccion;
                existente.Ciudad = configuracion.Ciudad;
                existente.Pais = configuracion.Pais;
                existente.CodigoPostal = configuracion.CodigoPostal;
                existente.RUC = configuracion.RUC;
                existente.Descripcion = configuracion.Descripcion;
                existente.FacebookUrl = configuracion.FacebookUrl;
                existente.InstagramUrl = configuracion.InstagramUrl;
                existente.TwitterUrl = configuracion.TwitterUrl;
                existente.LinkedInUrl = configuracion.LinkedInUrl;
                existente.TikTokUrl = configuracion.TikTokUrl;
                existente.TextoPiePagina = configuracion.TextoPiePagina;

                // Actualizar rutas de imágenes si fueron proporcionadas
                if (!string.IsNullOrEmpty(configuracion.RutaLogo))
                    existente.RutaLogo = configuracion.RutaLogo;

                if (!string.IsNullOrEmpty(configuracion.RutaFavicon))
                    existente.RutaFavicon = configuracion.RutaFavicon;

                if (!string.IsNullOrEmpty(configuracion.RutaImagenLogin))
                    existente.RutaImagenLogin = configuracion.RutaImagenLogin;

                if (!string.IsNullOrEmpty(configuracion.RutaImagenInstitucional))
                    existente.RutaImagenInstitucional = configuracion.RutaImagenInstitucional;

                if (!string.IsNullOrEmpty(configuracion.RutaBanner))
                    existente.RutaBanner = configuracion.RutaBanner;

                if (!string.IsNullOrEmpty(configuracion.RutaFondoLogin))
                    existente.RutaFondoLogin = configuracion.RutaFondoLogin;

                if (!string.IsNullOrEmpty(configuracion.RutaFondoDashboard))
                    existente.RutaFondoDashboard = configuracion.RutaFondoDashboard;

                if (!string.IsNullOrEmpty(configuracion.RutaFondoMenu))
                    existente.RutaFondoMenu = configuracion.RutaFondoMenu;

                existente.FechaActualizacion = DateTime.Now;

                if (esNuevoRegistro)
                {
                    await _unitOfWork.Configuraciones.AddAsync(existente);
                }
                else
                {
                    _unitOfWork.Configuraciones.Update(existente);
                }

                // Guardar auditoría
                foreach (var cambio in cambios)
                {
                    await _unitOfWork.ConfiguracionesAuditoria.AddAsync(cambio);
                }

                await _unitOfWork.CommitAsync();

                // Limpiar caché
                LimpiarCache();

                _logger.LogInformation($"Configuración actualizada por {nombreUsuario}. Se registraron {cambios.Count} cambios.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar configuración: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Registra los cambios realizados en la auditoría
        /// </summary>
        private void RegistrarCambioAuditoria(List<ConfiguracionAuditoria> cambios, ConfiguracionSistema existente, ConfiguracionSistema nueva, string usuarioId, string nombreUsuario)
        {
            var propiedades = typeof(ConfiguracionSistema).GetProperties();

            foreach (var propiedad in propiedades)
            {
                // Ignorar ID y fechas de auditoría
                if (propiedad.Name == "Id" || propiedad.Name == "FechaCreacion" || propiedad.Name == "FechaActualizacion")
                    continue;

                var valorAnterior = propiedad.GetValue(existente);
                var valorNuevo = propiedad.GetValue(nueva);

                // Solo registrar si hay cambio
                if (!Equals(valorAnterior, valorNuevo))
                {
                    cambios.Add(new ConfiguracionAuditoria
                    {
                        UsuarioId = usuarioId,
                        NombreUsuario = nombreUsuario,
                        PropiedadModificada = propiedad.Name,
                        ValorAnterior = valorAnterior?.ToString() ?? "NULL",
                        ValorNuevo = valorNuevo?.ToString() ?? "NULL",
                        FechaCambio = DateTime.Now
                    });
                }
            }
        }

        /// <summary>
        /// Guarda una imagen en wwwroot/uploads
        /// </summary>
        public async Task GuardarImagenAsync(string nombreCampo, IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return;

                // Crear carpeta uploads si no existe
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Validar extensión
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extensionArchivo = Path.GetExtension(archivo.FileName).ToLower();

                if (!extensionesPermitidas.Contains(extensionArchivo))
                    throw new InvalidOperationException($"Extensión {extensionArchivo} no permitida");

                // Validar tamaño máximo 5MB
                if (archivo.Length > 5 * 1024 * 1024)
                    throw new InvalidOperationException("Archivo demasiado grande (máximo 5MB)");

                // Generar nombre único
                var nombreUnico = $"{nombreCampo}_{Guid.NewGuid()}{extensionArchivo}";
                var rutaCompleta = Path.Combine(uploadsFolder, nombreUnico);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                _logger.LogInformation($"Imagen guardada: {nombreUnico}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar imagen: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Restablece la configuración a valores por defecto
        /// </summary>
        public async Task<ConfiguracionSistema> RestablecerConfiguracionPorDefectoAsync(string usuarioId, string nombreUsuario)
        {
            try
            {
                var existente = await _unitOfWork.Configuraciones.GetByIdAsync(1);

                if (existente == null)
                {
                    existente = new ConfiguracionSistema { Id = 1 };
                }

                // Guardar valores anteriores para auditoría
                var valoresAnteriores = new Dictionary<string, object?>
                {
                    { "NombreTienda", existente.NombreTienda },
                    { "ColorPrimario", existente.ColorPrimario },
                    { "ColorSecundario", existente.ColorSecundario },
                    { "ColorMenu", existente.ColorMenu },
                    { "ColorBotones", existente.ColorBotones },
                    { "ColorDashboard", existente.ColorDashboard },
                    { "ColorFondo", existente.ColorFondo },
                    { "TemaSeleccionado", existente.TemaSeleccionado },
                    { "TemaOscuro", existente.TemaOscuro }
                };

                // Restablecer valores por defecto
                existente.NombreTienda = "FashionStore";
                existente.ColorPrimario = "#667eea";
                existente.ColorSecundario = "#764ba2";
                existente.ColorMenu = "#2c3e50";
                existente.ColorBotones = "#667eea";
                existente.ColorDashboard = "#ecf0f1";
                existente.ColorFondo = "#f5f7fa";
                existente.TemaSeleccionado = "Fashion Store";
                existente.TemaOscuro = false;
                existente.RutaLogo = null;
                existente.RutaFavicon = null;
                existente.RutaImagenLogin = null;
                existente.RutaImagenInstitucional = null;
                existente.RutaBanner = null;
                existente.RutaFondoLogin = null;
                existente.RutaFondoDashboard = null;
                existente.RutaFondoMenu = null;
                existente.NombrePropietario = null;
                existente.Telefono = null;
                existente.Correo = null;
                existente.Direccion = null;
                existente.Ciudad = null;
                existente.Pais = null;
                existente.CodigoPostal = null;
                existente.RUC = null;
                existente.Descripcion = null;
                existente.FacebookUrl = null;
                existente.InstagramUrl = null;
                existente.TwitterUrl = null;
                existente.LinkedInUrl = null;
                existente.TikTokUrl = null;
                existente.TextoPiePagina = "&copy; 2025 FashionStore. Todos los derechos reservados.";
                existente.FechaActualizacion = DateTime.Now;

                _unitOfWork.Configuraciones.Update(existente);

                // Registrar en auditoría
                var auditoria = new ConfiguracionAuditoria
                {
                    UsuarioId = usuarioId,
                    NombreUsuario = nombreUsuario,
                    PropiedadModificada = "CONFIGURACION_COMPLETA",
                    ValorAnterior = "Valores anteriores",
                    ValorNuevo = "Valores por defecto",
                    FechaCambio = DateTime.Now,
                    Descripcion = "Se reestablecieron todos los valores a configuración por defecto"
                };

                await _unitOfWork.ConfiguracionesAuditoria.AddAsync(auditoria);
                await _unitOfWork.CommitAsync();

                // Limpiar caché
                LimpiarCache();

                _logger.LogInformation($"Configuración restablecida a valores por defecto por {nombreUsuario}");

                return existente;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al restablecer configuración: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Limpia el caché de configuración
        /// </summary>
        public void LimpiarCache()
        {
            _cache.Remove(CACHE_KEY);
            _logger.LogInformation("Caché de configuración limpiada");
        }
    }
}
