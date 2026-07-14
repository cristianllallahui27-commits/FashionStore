using FashionStore.Domain.Entities;
using FashionStore.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.Web.Controllers
{
    /// <summary>
    /// Controlador para administrar la configuración del sistema
    /// Solo accesible para usuarios con rol Administrador
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguracionSistemaService _configuracionService;
        private readonly ILogger<ConfiguracionController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> _roleManager;
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;

        public ConfiguracionController(
            IConfiguracionSistemaService configuracionService,
            ILogger<ConfiguracionController> logger,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager,
            FashionStore.Infrastructure.Context.FashionStoreDbContext context)
        {
            _configuracionService = configuracionService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        /// Obtiene el ID y nombre del usuario autenticado
        /// </summary>
        private (string Id, string Nombre) ObtenerDatosUsuario()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var nombreUsuario = User.Identity?.Name ?? "Desconocido";
            return (usuarioId, nombreUsuario);
        }

        /// <summary>
        /// Muestra la página de configuración del sistema
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var configuracion = await _configuracionService.ObtenerConfiguracionAsync();
                
                // Cargar usuarios para administración
                var usersList = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_userManager.Users);
                var userViewModels = new List<UsuarioViewModel>();
                foreach (var user in usersList)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userViewModels.Add(new UsuarioViewModel
                    {
                        Id = user.Id,
                        NombreUsuario = user.UserName ?? "",
                        Email = user.Email ?? "",
                        Activo = user.Activo,
                        Rol = roles.FirstOrDefault() ?? "Ninguno"
                    });
                }
                ViewBag.Usuarios = userViewModels;
                ViewBag.Descuentos = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.DescuentosAutorizados);

                return View(configuracion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar configuración: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(string Email, string Password, string Rol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Rol))
                {
                    TempData["Error"] = "Todos los campos del nuevo usuario son obligatorios.";
                    return RedirectToAction(nameof(Index));
                }

                var user = new ApplicationUser
                {
                    UserName = Email.Trim(),
                    Email = Email.Trim(),
                    EmailConfirmed = true,
                    Activo = true
                };

                var result = await _userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(Rol))
                    {
                        await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(Rol));
                    }
                    await _userManager.AddToRoleAsync(user, Rol);

                    // Si es vendedor, mantener consistencia en la tabla Vendedores
                    if (Rol == "Vendedor")
                    {
                        var existeVendedor = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(
                            _context.Set<Vendedor>(), v => v.Correo == Email);
                        if (!existeVendedor)
                        {
                            var name = Email.Split('@')[0];
                            var vendedor = new Vendedor
                            {
                                Nombres = name,
                                Apellidos = "Usuario",
                                DNI = "00000000",
                                Correo = Email,
                                Estado = true
                            };
                            await _context.Set<Vendedor>().AddAsync(vendedor);
                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["Success"] = $"Usuario '{Email}' creado exitosamente con el rol '{Rol}'.";
                }
                else
                {
                    TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear usuario: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUsuario(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var currentUserId = _userManager.GetUserId(User);
                if (user.Id == currentUserId)
                {
                    TempData["Error"] = "No puedes desactivar tu propio usuario administrador.";
                    return RedirectToAction(nameof(Index));
                }

                user.Activo = !user.Activo;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Vendedor"))
                    {
                        var vendedor = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                            _context.Set<Vendedor>(), v => v.Correo == user.Email);
                        if (vendedor != null)
                        {
                            vendedor.Estado = user.Activo;
                            _context.Set<Vendedor>().Update(vendedor);
                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["Success"] = $"Usuario '{user.Email}' {(user.Activo ? "activado" : "desactivado")} correctamente.";
                }
                else
                {
                    TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar estado del usuario: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string Rol { get; set; } = string.Empty;
    }

    /// <summary>
    /// API Controller para operaciones AJAX de configuración
    /// </summary>
    [Authorize(Roles = "Administrador")]
    [Route("api/configuracion")]
    [ApiController]
    public class ConfiguracionApiController : ControllerBase
    {
        private readonly IConfiguracionSistemaService _configuracionService;
        private readonly ILogger<ConfiguracionApiController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ConfiguracionApiController(
            IConfiguracionSistemaService configuracionService,
            ILogger<ConfiguracionApiController> logger,
            IWebHostEnvironment environment)
        {
            _configuracionService = configuracionService;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Obtiene el ID y nombre del usuario autenticado
        /// </summary>
        private (string Id, string Nombre) ObtenerDatosUsuario()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var nombreUsuario = User.Identity?.Name ?? "Desconocido";
            return (usuarioId, nombreUsuario);
        }

        /// <summary>
        /// Obtiene la configuración actual del sistema
        /// </summary>
        [HttpGet("obtener")]
        public async Task<IActionResult> ObtenerConfiguracion()
        {
            try
            {
                var configuracion = await _configuracionService.ObtenerConfiguracionAsync();
                return Ok(new { success = true, data = configuracion });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener configuración: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la configuración del sistema
        /// </summary>
        [HttpPost("actualizar")]
        public async Task<IActionResult> ActualizarConfiguracion([FromBody] ConfiguracionSistema configuracion)
        {
            try
            {
                if (configuracion == null)
                    return BadRequest(new { success = false, message = "Configuración inválida" });

                // Forzar el Id siempre a 1
                configuracion.Id = 1;

                var (usuarioId, nombreUsuario) = ObtenerDatosUsuario();

                await _configuracionService.ActualizarConfiguracionAsync(configuracion, usuarioId, nombreUsuario);

                return Ok(new { success = true, message = "Configuración actualizada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar configuración: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Carga un archivo de imagen y actualiza su ruta en la configuración
        /// </summary>
        [HttpPost("cargar-imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CargarImagen([FromForm] IFormFile archivo, [FromForm] string campo)
        {
            try
            {
                _logger.LogInformation("Iniciando carga de imagen. Campo: {Campo}, Archivo: {Archivo}, Tamaño: {Tamaño}", 
                    campo, archivo?.FileName, archivo?.Length);

                if (archivo == null || archivo.Length == 0)
                {
                    _logger.LogWarning("Archivo vacío recibido");
                    return BadRequest(new { success = false, message = "Archivo vacío" });
                }

                if (string.IsNullOrWhiteSpace(campo))
                {
                    _logger.LogWarning("Campo vacío recibido");
                    return BadRequest(new { success = false, message = "Debe indicar el campo de configuración" });
                }

                // Validaciones
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extensionArchivo = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extensionArchivo))
                {
                    _logger.LogWarning("Extensión no permitida: {Extension}", extensionArchivo);
                    return BadRequest(new { success = false, message = $"Extensión {extensionArchivo} no permitida" });
                }

                // Tamaño máximo 5MB
                if (archivo.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("Archivo demasiado grande: {Tamaño}MB", archivo.Length / (1024 * 1024));
                    return BadRequest(new { success = false, message = "Archivo muy grande (máx 5MB)" });
                }

                // Crear carpeta uploads
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Carpeta uploads creada: {Ruta}", uploadsFolder);
                }

                // Generar nombre único
                var nombreUnico = $"{campo}_{Guid.NewGuid()}{extensionArchivo}";
                var rutaCompleta = Path.Combine(uploadsFolder, nombreUnico);

                // Guardar archivo
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }
                _logger.LogInformation("Archivo guardado: {RutaCompleta}", rutaCompleta);

                // Actualizar configuración con la ruta relativa
                var rutaRelativa = $"/uploads/{nombreUnico}";
                var configuracion = await _configuracionService.ObtenerConfiguracionAsync();

                // Asignar la ruta según el campo
                switch (campo.Trim().ToLowerInvariant())
                {
                    case "logo":
                        configuracion.RutaLogo = rutaRelativa;
                        break;
                    case "favicon":
                        configuracion.RutaFavicon = rutaRelativa;
                        break;
                    case "imagen_login":
                        configuracion.RutaImagenLogin = rutaRelativa;
                        break;
                    case "imagen_institucional":
                        configuracion.RutaImagenInstitucional = rutaRelativa;
                        break;
                    case "banner":
                        configuracion.RutaBanner = rutaRelativa;
                        break;
                    case "fondo_login":
                        configuracion.RutaFondoLogin = rutaRelativa;
                        break;
                    case "fondo_dashboard":
                        configuracion.RutaFondoDashboard = rutaRelativa;
                        break;
                    case "fondo_menu":
                        configuracion.RutaFondoMenu = rutaRelativa;
                        break;
                    default:
                        _logger.LogWarning("Campo de imagen no soportado: {Campo}", campo);
                        return BadRequest(new { success = false, message = "Campo de imagen no soportado" });
                }

                var (usuarioId, nombreUsuario) = ObtenerDatosUsuario();
                
                try
                {
                    await _configuracionService.ActualizarConfiguracionAsync(configuracion, usuarioId, nombreUsuario);
                    _logger.LogInformation("Imagen guardada exitosamente en BD. Campo: {Campo}, Ruta: {Ruta}", campo, rutaRelativa);
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Error al actualizar BD después de guardar imagen: {Mensaje}", updateEx.Message);
                    // Continuar - la imagen está guardada en disco aunque la BD falle
                }

                return Ok(new
                {
                    success = true,
                    message = "Imagen cargada correctamente",
                    ruta = rutaRelativa,
                    nombreArchivo = nombreUnico
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar imagen: {Mensaje}", ex.Message);
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Restablece la configuración a valores por defecto
        /// </summary>
        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerConfiguracion()
        {
            try
            {
                var (usuarioId, nombreUsuario) = ObtenerDatosUsuario();
                var configuracion = await _configuracionService.RestablecerConfiguracionPorDefectoAsync(usuarioId, nombreUsuario);

                return Ok(new { success = true, message = "Configuración restablecida correctamente", data = configuracion });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al restablecer configuración: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
