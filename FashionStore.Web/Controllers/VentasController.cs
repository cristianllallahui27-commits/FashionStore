using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Web.Services;
using FashionStore.Web.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador,Vendedor")]
    public class VentasController : Controller
    {
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguracionSistemaService _configService;
        private readonly ILogger<VentasController> _logger;

        public VentasController(
            FashionStore.Infrastructure.Context.FashionStoreDbContext context,
            IUnitOfWork unitOfWork,
            IConfiguracionSistemaService configService,
            ILogger<VentasController> logger)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _configService = configService;
            _logger = logger;
        }

        // =========================================
        // INDEX
        // =========================================

        public async Task<IActionResult> Index()
        {
            try
            {
                var ventas = await _unitOfWork.Ventas.GetAllAsync();
                
                IEnumerable<Venta> query = ventas
                    .AsEnumerable();

                // Vendedor solo ve sus propias ventas
                if (User.IsInRole("Vendedor") && !User.IsInRole("Administrador"))
                {
                    var userEmail = User.Identity?.Name;
                    var vendedores = await _unitOfWork.Vendedores.FindAsync(v => v.Correo == userEmail);
                    var vendedor = vendedores.FirstOrDefault();
                    
                    if (vendedor != null)
                    {
                        query = query.Where(v => v.VendedorId == vendedor.Id);
                        _logger.LogInformation("Vendedor accessing sales. VendedorId: {VendedorId}, Email: {Email}", vendedor.Id, userEmail);
                    }
                    else
                    {
                        query = query.Where(v => false); // ninguna si no tiene vendedor asociado
                        _logger.LogWarning("Vendedor without associated vendor record. Email: {Email}", userEmail);
                    }
                }

                var ventasList = query.OrderByDescending(v => v.Fecha).ToList();

                var hoy = DateTime.Now.Date;
                var ventasHoy = ventasList.Where(v => v.Fecha.Date == hoy).ToList();

                var model = new VentasIndexViewModel
                {
                    Ventas = ventasList,
                    VentasHoy = ventasHoy.Count,
                    IngresosHoy = ventasHoy.Sum(v => v.Total),
                    TotalVentas = ventasList.Count,
                    IngresosTotal = ventasList.Sum(v => v.Total)
                };

                _logger.LogInformation("Sales Index loaded. TotalSales: {TotalSales}, SalesHoy: {SalesHoy}, RevenueToday: {RevenueToday:F2}", 
                    model.TotalVentas, model.VentasHoy, model.IngresosHoy);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sales index");
                return View(new VentasIndexViewModel());
            }
        }

        // =========================================
        // CREATE (Pagina POS completa)
        // =========================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Ensure generic client exists
                var clientesGenerico = await _unitOfWork.Clientes.FindAsync(c => c.DNI == "00000000");
                var clienteGenerico = clientesGenerico.FirstOrDefault();
                
                if (clienteGenerico == null)
                {
                    clienteGenerico = new Cliente
                    {
                        NombreCompleto = "Cliente Genérico",
                        DNI = "00000000",
                        Telefono = "-",
                        Direccion = "-"
                    };
                    await _unitOfWork.Clientes.AddAsync(clienteGenerico);
                    await _unitOfWork.CommitAsync();
                    _logger.LogInformation("Generic client created. ClienteId: {ClienteId}", clienteGenerico.Id);
                }

                var clientes = await _unitOfWork.Clientes.GetAllAsync();
                var vendedores = await _unitOfWork.Vendedores.FindAsync(v => v.Estado);
                var metodosPago = await _unitOfWork.MetodosPago.GetAllAsync();
                var descuentos = await _unitOfWork.DescuentosAutorizados.FindAsync(d => d.Activo);

                // ✅ DETECCIÓN AUTOMÁTICA DE VENDEDOR DESDE SESIÓN
                int? vendedorLogueadoId = null;
                string vendedorLogueadoNombre = "";
                var usuarioEmail = User.Identity?.Name;
                
                if (!string.IsNullOrEmpty(usuarioEmail))
                {
                    var vendedoresLogueado = await _unitOfWork.Vendedores.FindAsync(v => v.Correo == usuarioEmail && v.Estado);
                    var vendedor = vendedoresLogueado.FirstOrDefault();
                    
                    if (vendedor != null)
                    {
                        vendedorLogueadoId = vendedor.Id;
                        vendedorLogueadoNombre = $"{vendedor.Nombres} {vendedor.Apellidos}";
                        _logger.LogInformation("Auto-detected vendor from session. VendedorId: {VendedorId}, Email: {Email}", vendedor.Id, usuarioEmail);
                    }
                    else if (User.IsInRole("Administrador"))
                    {
                        // Si es Admin y NO hay vendedor, buscar vendedor admin (DNI ADMIN0001)
                        var vendedoresAdmin = await _unitOfWork.Vendedores.FindAsync(v => v.DNI == "ADMIN0001" && v.Estado);
                        var vendedorAdmin = vendedoresAdmin.FirstOrDefault();
                        
                        if (vendedorAdmin != null)
                        {
                            vendedorLogueadoId = vendedorAdmin.Id;
                            vendedorLogueadoNombre = $"{vendedorAdmin.Nombres} {vendedorAdmin.Apellidos}";
                            _logger.LogInformation("Auto-detected admin vendor from system. VendedorId: {VendedorId}", vendedorAdmin.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Admin user but no admin vendor record found. Email: {Email}", usuarioEmail);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("User logged in but no vendor record found. Email: {Email}", usuarioEmail);
                    }
                }

                var vm = new VentaCreateViewModel
                {
                    Clientes = clientes.OrderBy(c => c.NombreCompleto)
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.NombreCompleto} (DNI: {c.DNI})" })
                        .ToList(),
                    Vendedores = vendedores.OrderBy(v => v.Nombres)
                        .Select(v => new SelectListItem { Value = v.Id.ToString(), Text = $"{v.Nombres} {v.Apellidos}" })
                        .ToList(),
                    MetodosPago = metodosPago
                        .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nombre })
                        .ToList(),
                    DescuentosActivos = descuentos.Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = $"{d.Nombre} - {(d.Tipo == TipoDescuento.Porcentaje ? d.Valor + "%" : "S/." + d.Valor)}"
                    }).ToList(),
                    DescuentosJson = System.Text.Json.JsonSerializer.Serialize(descuentos.Select(d => new {
                        d.Id,
                        d.Nombre,
                        Tipo = d.Tipo.ToString(),
                        d.Valor
                    })),
                    // ✅ Pasar datos del vendedor logueado
                    VendedorLogueadoId = vendedorLogueadoId,
                    VendedorLogueadoNombre = vendedorLogueadoNombre,
                    VendedorAutoDetectado = vendedorLogueadoId.HasValue,
                    VendedorPreseleccionadoId = vendedorLogueadoId
                };

                _logger.LogInformation("POS Create page loaded. Clientes: {ClienteCount}, Vendedores: {VendedorCount}, VendedorDetectado: {VendedorDetected}", 
                    vm.Clientes.Count, vm.Vendedores.Count, vm.VendedorAutoDetectado);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading POS Create page");
                throw;
            }
        }

        // =========================================
        // DETAILS - Informe/Comprobante de venta
        // =========================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ventas = await _unitOfWork.Ventas.FindAsync(v => v.Id == id);
            var venta = ventas.FirstOrDefault();

            if (venta == null) return NotFound();

            var config = await _configService.ObtenerConfiguracionAsync();

            var vm = new VentaDetalleViewModel
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                ClienteNombre = venta.Cliente?.NombreCompleto ?? "-",
                ClienteDNI = venta.Cliente?.DNI ?? "-",
                ClienteTelefono = venta.Cliente?.Telefono ?? "-",
                VendedorNombre = venta.Vendedor != null
                    ? $"{venta.Vendedor.Nombres} {venta.Vendedor.Apellidos}"
                    : "-",
                MetodoPagoNombre = venta.MetodoPago?.Nombre ?? "-",
                MontoRecibido = venta.MontoRecibido,
                Vuelto = venta.Vuelto,
                Descuento = venta.Descuento,
                DescuentoNombre = venta.DescuentoAutorizado?.Nombre,
                Total = venta.Total,
                NombreTienda = config.NombreTienda,
                TelefonoTienda = config.Telefono,
                CorreoTienda = config.Correo,
                DireccionTienda = config.Direccion,
                RutaLogo = config.RutaLogo,
                Detalles = (venta.DetalleVentas ?? new List<DetalleVenta>())
                    .Select(d => new DetalleVentaItem
                    {
                        NombrePrenda = d.Prenda?.Nombre ?? "-",
                        CodigoBarra = d.Prenda?.CodigoBarra ?? "",
                        Talla = d.Prenda?.Talla ?? "-",
                        Color = d.Prenda?.Color ?? "-",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.Precio,
                        Subtotal = d.Subtotal
                    }).ToList()
            };

            return View(vm);
        }

        // =========================================
        // DESCARGAR EXCEL (Una Venta)
        // =========================================

        [HttpGet]
        public async Task<IActionResult> DescargarExcel(int id)
        {
            var ventas = await _unitOfWork.Ventas.FindAsync(v => v.Id == id);
            var venta = ventas.FirstOrDefault();

            if (venta == null) return NotFound();

            var config = await _configService.ObtenerConfiguracionAsync();

            // Retornar datos como JSON para ser procesados por cliente
            var detallesExcel = (venta.DetalleVentas ?? new List<DetalleVenta>())
                .Select(d => new
                {
                    Producto = d.Prenda?.Nombre ?? "-",
                    CodigoBarra = d.Prenda?.CodigoBarra ?? "",
                    TallaColor = $"{d.Prenda?.Talla} / {d.Prenda?.Color}",
                    PrecioUnitario = d.Precio.ToString("F2"),
                    Cantidad = d.Cantidad,
                    Subtotal = d.Subtotal.ToString("F2")
                })
                .ToList();

            decimal subtotalExcel = venta.DetalleVentas?.Sum(d => d.Subtotal) ?? 0;

            var descuentoInfo = venta.Descuento > 0
                ? new
                {
                    Nombre = !string.IsNullOrEmpty(venta.DescuentoAutorizado?.Nombre)
                        ? $"Descuento ({venta.DescuentoAutorizado.Nombre}):"
                        : "Descuento:",
                    Valor = (-venta.Descuento).ToString("F2")
                }
                : null;

            var response = new
            {
                NombreTienda = config.NombreTienda,
                NumeroVenta = $"#{venta.Id:D4}",
                Fecha = venta.Fecha.ToString("dd/MM/yyyy HH:mm"),
                Cliente = venta.Cliente?.NombreCompleto ?? "-",
                DNI = venta.Cliente?.DNI ?? "-",
                Vendedor = venta.Vendedor != null ? $"{venta.Vendedor.Nombres} {venta.Vendedor.Apellidos}" : "-",
                MetodoPago = venta.MetodoPago?.Nombre ?? "-",
                Detalles = detallesExcel,
                Subtotal = subtotalExcel.ToString("F2"),
                Descuento = descuentoInfo,
                Total = venta.Total.ToString("F2"),
                MontoRecibido = venta.MontoRecibido?.ToString("F2"),
                Vuelto = (venta.Vuelto ?? 0).ToString("F2")
            };

            return Json(response);
        }

        // =========================================
        // DESCARGAR EXCEL GENERAL (Todas las ventas)
        // =========================================
        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var ventas = await _unitOfWork.Ventas.GetAllAsync();
            var ventasList = ventas.OrderByDescending(v => v.Fecha).ToList();

            // Retornar datos como JSON para ser procesados por cliente
            var ventasData = ventasList.Select(venta => new
            {
                IdVenta = $"#{venta.Id:D4}",
                Cliente = venta.Cliente?.NombreCompleto ?? "Público General",
                Fecha = venta.Fecha.ToString("dd/MM/yyyy HH:mm"),
                MetodoPago = venta.MetodoPago?.Nombre ?? "Efectivo",
                Subtotal = (venta.Total + venta.Descuento).ToString("F2"),
                Descuento = venta.Descuento.ToString("F2"),
                DescuentoAplicado = venta.DescuentoAutorizado?.Nombre ?? "-",
                Total = venta.Total.ToString("F2")
            }).ToList();

            var response = new
            {
                titulo = "Reporte de Ventas",
                totalRegistros = ventasList.Count,
                datos = ventasData
            };

            return Json(response);
        }


        // =========================================
        // DESCARGAR PDF (vista imprimible)
        // =========================================

        [HttpGet]
        public async Task<IActionResult> Imprimir(int id)
        {
            var ventas = await _unitOfWork.Ventas.FindAsync(v => v.Id == id);
            var venta = ventas.FirstOrDefault();

            if (venta == null) return NotFound();

            var config = await _configService.ObtenerConfiguracionAsync();

            var vm = new VentaDetalleViewModel
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                ClienteNombre = venta.Cliente?.NombreCompleto ?? "-",
                ClienteDNI = venta.Cliente?.DNI ?? "-",
                ClienteTelefono = venta.Cliente?.Telefono ?? "-",
                VendedorNombre = venta.Vendedor != null
                    ? $"{venta.Vendedor.Nombres} {venta.Vendedor.Apellidos}"
                    : "-",
                MetodoPagoNombre = venta.MetodoPago?.Nombre ?? "-",
                MontoRecibido = venta.MontoRecibido,
                Vuelto = venta.Vuelto,
                Descuento = venta.Descuento,
                DescuentoNombre = venta.DescuentoAutorizado?.Nombre,
                Total = venta.Total,
                NombreTienda = config.NombreTienda,
                TelefonoTienda = config.Telefono,
                CorreoTienda = config.Correo,
                DireccionTienda = config.Direccion,
                RutaLogo = config.RutaLogo,
                Detalles = (venta.DetalleVentas ?? new List<DetalleVenta>())
                    .Select(d => new DetalleVentaItem
                    {
                        NombrePrenda = d.Prenda?.Nombre ?? "-",
                        CodigoBarra = d.Prenda?.CodigoBarra ?? "",
                        Talla = d.Prenda?.Talla ?? "-",
                        Color = d.Prenda?.Color ?? "-",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.Precio,
                        Subtotal = d.Subtotal
                    }).ToList()
            };

            return View(vm);
        }

        // =========================================
        // DASHBOARD
        // =========================================

        public async Task<IActionResult> Dashboard()
        {
            var ventas = await _unitOfWork.Ventas.GetAllAsync();
            var detalles = await _unitOfWork.DetalleVentas.GetAllAsync();

            var ventasList = ventas.ToList();
            var detallesList = detalles.ToList();

            var ventasPorMes = ventasList
                .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
                .Select(g => new
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Total = g.Count(),
                    Ingresos = g.Sum(x => x.Total)
                })
                .OrderBy(x => x.Label)
                .ToList();

            var ventasPorCategoria = detallesList
                .GroupBy(d => d.Prenda?.Categoria?.Nombre ?? "Sin categoria")
                .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Subtotal) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var model = new DashboardViewModel
            {
                TotalVentas = ventasList.Count,
                TotalClientes = ventasList.Select(v => v.ClienteId).Distinct().Count(),
                TotalIngresos = ventasList.Sum(v => v.Total),
                SalesChartLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                    ventasPorMes.Select(x => x.Label).ToArray()),
                SalesChartDataJson = System.Text.Json.JsonSerializer.Serialize(
                    ventasPorMes.Select(x => x.Ingresos).ToArray()),
                PrendasByCategoryLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                    ventasPorCategoria.Select(x => x.Categoria).ToArray()),
                PrendasByCategoryDataJson = System.Text.Json.JsonSerializer.Serialize(
                    ventasPorCategoria.Select(x => x.Total).ToArray())
            };

            return View(model);
        }

        // =========================================
        // API - PRODUCTOS DISPONIBLES
        // =========================================

        [HttpGet("api/productos-disponibles")]
        public async Task<IActionResult> ApiProductosDisponibles()
        {
            try
            {
                var prendas = await _unitOfWork.Prendas.FindAsync(p => p.Stock > 0);
                var productos = prendas.Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Precio,
                    p.Stock,
                    p.Color,
                    p.Talla,
                    p.CodigoBarra,
                    p.ImagenUrl,
                    Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoria"
                }).ToList();

                return Json(new { exito = true, datos = productos });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar productos disponibles: {ex.Message}");
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - BUSCAR PRODUCTOS
        // =========================================

        [HttpGet("api/buscar/{nombre}")]
        public async Task<IActionResult> ApiBuscar(string nombre)
        {
            try
            {
                // Validación 1: nombre no sea nulo o vacío
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest(new { exito = false, mensaje = "El nombre de búsqueda no puede estar vacío." });

                // Validación 2: longitud > 2 caracteres
                if (nombre.Trim().Length <= 2)
                    return BadRequest(new { exito = false, mensaje = "El nombre de búsqueda debe tener más de 2 caracteres." });

                var search = nombre.Trim().ToLower();
                var prendas = await _unitOfWork.Prendas.FindAsync(p => 
                    (p.Nombre.ToLower().Contains(search) || (p.CodigoBarra != null && p.CodigoBarra.ToLower().Contains(search))) && p.Stock > 0);
                
                var productos = prendas
                    .OrderBy(p => p.Id)
                    .Take(20)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.Precio,
                        p.Stock,
                        p.Color,
                        p.Talla,
                        p.CodigoBarra,
                        p.ImagenUrl,
                        Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoria"
                    })
                    .ToList();

                return Json(new { exito = true, datos = productos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - BUSCAR PRODUCTO POR CÓDIGO DE BARRAS
        // =========================================

        [HttpGet("api/buscar-codigo/{codigo}")]
        public async Task<IActionResult> ApiBuscarCodigo(string codigo)
        {
            try
            {
                // Validación: código no sea nulo o vacío
                if (string.IsNullOrWhiteSpace(codigo))
                    return BadRequest(new { exito = false, mensaje = "El código de barras no puede estar vacío." });

                var prendas = await _unitOfWork.Prendas.FindAsync(p => p.CodigoBarra == codigo.Trim());
                var producto = prendas.FirstOrDefault();

                if (producto == null)
                    return BadRequest(new { exito = false, mensaje = $"Producto con código de barras '{codigo.Trim()}' no encontrado." });

                return Json(new
                {
                    exito = true,
                    datos = new
                    {
                        producto.Id,
                        producto.Nombre,
                        producto.Precio,
                        producto.Stock,
                        producto.Color,
                        producto.Talla,
                        producto.CodigoBarra,
                        Categoria = producto.Categoria != null ? producto.Categoria.Nombre : "Sin categoría",
                        ImagenUrl = producto.ImagenUrl
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - VENTAS RECIENTES
        // =========================================

        [HttpGet("api/ventas-recientes")]
        public async Task<IActionResult> ApiVentasRecientes()
        {
            try
            {
                var ventas = await _unitOfWork.Ventas.GetAllAsync();
                var ventasList = ventas
                    .OrderByDescending(v => v.Fecha)
                    .Take(50)
                    .Select(v => new
                    {
                        v.Id,
                        Fecha = v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        Cliente = v.Cliente != null ? v.Cliente.NombreCompleto : "-",
                        MetodoPago = v.MetodoPago != null ? v.MetodoPago.Nombre : "-",
                        Total = v.Total
                    })
                    .ToList();

                return Json(new { exito = true, datos = ventasList });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - CLIENTES DISPONIBLES
        // =========================================

        [HttpGet("api/clientes-disponibles")]
        public async Task<IActionResult> ApiClientesDisponibles()
        {
            try
            {
                var clientes = await _unitOfWork.Clientes.GetAllAsync();
                var datos = clientes.OrderBy(c => c.NombreCompleto).Select(c => new
                {
                    c.Id,
                    nombreCompleto = c.NombreCompleto,
                    dni = c.DNI
                }).ToList();

                return Json(new { exito = true, datos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - VENDEDORES DISPONIBLES
        // =========================================

        [HttpGet("api/vendedores-disponibles")]
        public async Task<IActionResult> ApiVendedoresDisponibles()
        {
            try
            {
                var vendedores = await _unitOfWork.Vendedores.FindAsync(v => v.Estado);
                var datos = vendedores.Select(v => new
                {
                    v.Id,
                    nombreCompleto = $"{v.Nombres} {v.Apellidos}",
                    v.DNI
                }).ToList();

                return Json(new { exito = true, datos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - METODOS DE PAGO
        // =========================================

        [HttpGet("api/metodos-pago")]
        public async Task<IActionResult> ApiMetodosPago()
        {
            try
            {
                var metodos = await _unitOfWork.MetodosPago.GetAllAsync();
                var datos = metodos.Select(m => new { m.Id, m.Nombre }).ToList();
                return Json(new { exito = true, datos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - PRODUCTOS AGOTANDOSE
        // =========================================

        [HttpGet("api/productos-agotandose")]
        public async Task<IActionResult> ApiProductosAgotandose()
        {
            try
            {
                var prendas = await _unitOfWork.Prendas.FindAsync(p => p.Stock > 0 && p.Stock <= 5);
                var productos = prendas
                    .OrderBy(p => p.Stock)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.Stock,
                        Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoria",
                        Estado = p.Stock <= 2 ? "Critico" : "Bajo"
                    })
                    .ToList();

                return Json(new { exito = true, datos = productos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - DESCUENTOS ACTIVOS (para POS)
        // =========================================

        [HttpGet("api/descuentos-activos")]
        public async Task<IActionResult> ApiDescuentosActivos()
        {
            try
            {
                var descuentos = await _unitOfWork.DescuentosAutorizados.FindAsync(d => d.Activo);
                var datos = descuentos.Select(d => new
                {
                    d.Id,
                    d.Nombre,
                    tipo = d.Tipo.ToString(),
                    d.Valor
                }).ToList();

                return Json(new { exito = true, datos });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - CREAR CLIENTE RAPIDO (RF-01)
        // =========================================

        [HttpPost("api/cliente-rapido")]
        public async Task<IActionResult> ApiClienteRapido([FromBody] ClienteRapidoRequest request)
        {
            try
            {
                // Validación 1: request no sea nulo
                if (request == null)
                {
                    _logger.LogWarning("Attempt to create quick client with null request");
                    return BadRequest(new { exito = false, mensaje = "Los datos del cliente no pueden estar vacíos." });
                }

                // Validación 2: NombreCompleto no sea vacío
                if (string.IsNullOrWhiteSpace(request.NombreCompleto))
                {
                    _logger.LogWarning("Attempt to create quick client without name");
                    return BadRequest(new { exito = false, mensaje = "El nombre completo es obligatorio." });
                }

                // Validación 3: DNI no sea vacío
                if (string.IsNullOrWhiteSpace(request.DNI))
                {
                    _logger.LogWarning("Attempt to create quick client without DNI");
                    return BadRequest(new { exito = false, mensaje = "El DNI es obligatorio." });
                }

                // Validación 4: DNI tenga formato válido (8 dígitos)
                var dniTrimmed = request.DNI.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(dniTrimmed, @"^\d{8}$"))
                {
                    _logger.LogWarning("Invalid DNI format: {DNI}", dniTrimmed);
                    return BadRequest(new { exito = false, mensaje = "El DNI debe contener exactamente 8 dígitos." });
                }

                // Validación 5: Email sea válido si se proporciona
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(request.Email.Trim());
                    }
                    catch
                    {
                        _logger.LogWarning("Invalid email format: {Email}", request.Email);
                        return BadRequest(new { exito = false, mensaje = "El formato de email no es válido." });
                    }
                }

                // Verificar DNI duplicado
                var clientesExistentes = await _unitOfWork.Clientes.FindAsync(c => c.DNI == dniTrimmed);
                var existente = clientesExistentes.FirstOrDefault();

                if (existente != null)
                {
                    _logger.LogWarning("Attempt to create client with duplicate DNI: {DNI}", dniTrimmed);
                    return BadRequest(new { exito = false, mensaje = $"Ya existe un cliente con DNI {dniTrimmed}." });
                }

                var cliente = new Cliente
                {
                    NombreCompleto = request.NombreCompleto.Trim(),
                    DNI = dniTrimmed,
                    Email = request.Email?.Trim(),
                    Telefono = request.Telefono?.Trim() ?? "",
                    Direccion = request.Direccion?.Trim() ?? ""
                };

                await _unitOfWork.Clientes.AddAsync(cliente);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Quick client created successfully. ClienteId: {ClienteId}, DNI: {DNI}, Nombre: {Nombre}", 
                    cliente.Id, cliente.DNI, cliente.NombreCompleto);

                return Json(new
                {
                    exito = true,
                    mensaje = "Cliente creado exitosamente.",
                    datos = new
                    {
                        id = cliente.Id,
                        nombreCompleto = cliente.NombreCompleto,
                        dni = cliente.DNI
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quick client");
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // API - REGISTRAR VENTA (RF-06)
        // =========================================

        [HttpPost("api/registrar-venta")]
        public async Task<IActionResult> ApiRegistrarVenta([FromBody] RegistrarVentaRequest request)
        {
            try
            {
                // Validación 1: request no sea nulo
                if (request == null)
                {
                    _logger.LogWarning("Attempt to register sale with null request");
                    return BadRequest(new { exito = false, mensaje = "Los datos de la venta no pueden estar vacíos." });
                }

                // Validación 2: Detalles no esté vacío
                if (request.Detalles == null || !request.Detalles.Any())
                {
                    _logger.LogWarning("Attempt to register sale with no details");
                    return BadRequest(new { exito = false, mensaje = "La venta debe contener al menos un producto." });
                }

                // Validación 3: Validar ModelState
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    _logger.LogWarning("Sale registration with invalid ModelState. Errors: {@Errors}", errores);
                    return BadRequest(new { exito = false, mensaje = "Datos inválidos en la solicitud.", detalles = errores });
                }

                var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var vendedor = User.Identity?.Name;

                _logger.LogInformation("Starting sale registration. ClienteId: {ClienteId}, VendedorId: {VendedorId}, Usuario: {Usuario}", 
                    request.ClienteId, request.VendedorId, vendedor);

                var ventaId = await RegistrarVentaInterno(request, usuarioId);

                _logger.LogInformation("Sale registered successfully. VentaId: {VentaId}, Total: {Total}, Descuento: {Descuento}", 
                    ventaId, request.Detalles.Count, request.DescuentoAutorizadoId);

                return Json(new { exito = true, mensaje = "Venta registrada exitosamente.", datos = new { ventaId } });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation during sale registration: {Message}", ex.Message);
                return BadRequest(new { exito = false, mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering sale");
                return Json(new { exito = false, mensaje = $"Error: {ex.Message}" });
            }
        }

        // =========================================
        // API - VALIDAR VENTA
        // =========================================

        [HttpPost("api/validar-venta")]
        public async Task<IActionResult> ApiValidarVenta([FromBody] RegistrarVentaRequest request)
        {
            try
            {
                // Validación 1: request no sea nulo
                if (request == null)
                {
                    _logger.LogWarning("Attempt to validate sale with null request");
                    return BadRequest(new { exito = false, mensaje = "Los datos de la venta no pueden estar vacíos." });
                }

                // Validación 2: Detalles no esté vacío
                if (request.Detalles == null || !request.Detalles.Any())
                {
                    _logger.LogWarning("Attempt to validate sale with no details");
                    return BadRequest(new { exito = false, mensaje = "La venta debe contener al menos un producto." });
                }

                var detalles = request.Detalles.Select(d => new DetalleVentaDTO
                {
                    PrendaId = d.PrendaId,
                    Cantidad = d.Cantidad,
                    Precio = 0
                }).ToList();

                _logger.LogInformation("Starting sale validation. ClienteId: {ClienteId}, VendedorId: {VendedorId}, DetalleCount: {DetalleCount}", 
                    request.ClienteId, request.VendedorId, detalles.Count);

                var (exito, mensaje) = await ValidarVentaInterno(request.ClienteId, request.VendedorId,
                    request.MetodoPagoId, detalles);

                if (!exito)
                    _logger.LogWarning("Sale validation failed: {Mensaje}", mensaje);

                return Json(new { exito, mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating sale");
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // =========================================
        // METODOS PRIVADOS
        // =========================================

        private async Task<int> RegistrarVentaInterno(RegistrarVentaRequest request, string? usuarioAutenticadoId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar entidades
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(request.ClienteId);
                if (cliente == null) 
                {
                    _logger.LogError("Cliente not found. ClienteId: {ClienteId}", request.ClienteId);
                    throw new InvalidOperationException("Cliente no encontrado.");
                }

                var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(request.VendedorId);
                if (vendedor == null)
                {
                    _logger.LogError("Vendedor not found. VendedorId: {VendedorId}", request.VendedorId);
                    throw new InvalidOperationException("Vendedor no encontrado.");
                }

                var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(request.MetodoPagoId);
                if (metodoPago == null)
                {
                    _logger.LogError("MetodoPago not found. MetodoPagoId: {MetodoPagoId}", request.MetodoPagoId);
                    throw new InvalidOperationException("Metodo de pago no encontrado.");
                }

                if (!request.Detalles.Any())
                {
                    _logger.LogError("Sale has no details");
                    throw new InvalidOperationException("La venta debe tener al menos un producto.");
                }

                // Calcular subtotal DESDE BD (no confiar en precio del cliente)
                decimal subtotal = 0;
                var detallesConPrecio = new List<(DetalleVentaRequest req, Prenda prenda)>();

                foreach (var det in request.Detalles)
                {
                    if (det.Cantidad <= 0)
                    {
                        _logger.LogError("Invalid quantity. PrendaId: {PrendaId}, Cantidad: {Cantidad}", det.PrendaId, det.Cantidad);
                        throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
                    }

                    var prenda = await _unitOfWork.Prendas.GetByIdAsync(det.PrendaId);
                    if (prenda == null)
                    {
                        _logger.LogError("Prenda not found. PrendaId: {PrendaId}", det.PrendaId);
                        throw new InvalidOperationException($"Producto ID {det.PrendaId} no encontrado.");
                    }
                    if (prenda.Stock < det.Cantidad)
                    {
                        _logger.LogWarning("Insufficient stock for product. Prenda: {Prenda}, Available: {Available}, Requested: {Requested}", 
                            prenda.Nombre, prenda.Stock, det.Cantidad);
                        throw new InvalidOperationException($"Stock insuficiente para {prenda.Nombre}. Disponible: {prenda.Stock}, solicitado: {det.Cantidad}.");
                    }

                    subtotal += prenda.Precio * det.Cantidad;
                    detallesConPrecio.Add((det, prenda));
                }

                // Validar y aplicar descuento
                decimal descuentoAplicado = 0;
                int? descuentoIdAplicado = null;

                if (request.DescuentoAutorizadoId.HasValue && request.DescuentoAutorizadoId.Value > 0)
                {
                    var descuentos = await _unitOfWork.DescuentosAutorizados.FindAsync(d => 
                        d.Id == request.DescuentoAutorizadoId.Value && d.Activo);
                    var descuento = descuentos.FirstOrDefault();

                    if (descuento == null)
                    {
                        _logger.LogWarning("Authorized discount not found or inactive. DescuentoId: {DescuentoId}", request.DescuentoAutorizadoId.Value);
                        throw new InvalidOperationException("El descuento seleccionado no es válido o no está activo.");
                    }

                    descuentoAplicado = descuento.Tipo == TipoDescuento.Porcentaje
                        ? Math.Round(subtotal * descuento.Valor / 100, 2)
                        : descuento.Valor;

                    descuentoIdAplicado = descuento.Id;
                    _logger.LogInformation("Discount applied. Nombre: {Nombre}, Tipo: {Tipo}, Valor: {Valor}, DescuentoAplicado: {DescuentoAplicado}", 
                        descuento.Nombre, descuento.Tipo, descuento.Valor, descuentoAplicado);
                }
                else if (request.TipoDescuento != null && request.TipoDescuento != "Ninguno" && request.DescuentoManualValor > 0)
                {
                    // Validar si el usuario tiene rol de Administrador
                    if (!User.IsInRole("Administrador"))
                    {
                        _logger.LogWarning("Unauthorized manual discount attempt by user: {Usuario}", User.Identity?.Name);
                        throw new InvalidOperationException("El vendedor no está autorizado a aplicar descuentos manuales libres sin permiso.");
                    }

                    if (request.TipoDescuento == "Porcentaje")
                    {
                        if (request.DescuentoManualValor < 0 || request.DescuentoManualValor > 100)
                        {
                            _logger.LogError("Invalid discount percentage: {Porcentaje}", request.DescuentoManualValor);
                            throw new InvalidOperationException("El porcentaje de descuento debe estar entre 0% y 100%.");
                        }
                        descuentoAplicado = Math.Round(subtotal * request.DescuentoManualValor / 100, 2);
                    }
                    else if (request.TipoDescuento == "MontoFijo" || request.TipoDescuento == "Soles")
                    {
                        if (request.DescuentoManualValor < 0)
                        {
                            _logger.LogError("Negative discount amount: {Monto}", request.DescuentoManualValor);
                            throw new InvalidOperationException("El monto de descuento no puede ser negativo.");
                        }
                        descuentoAplicado = request.DescuentoManualValor;
                    }
                    _logger.LogInformation("Manual discount applied by admin. Tipo: {Tipo}, Valor: {Valor}, DescuentoAplicado: {DescuentoAplicado}", 
                        request.TipoDescuento, request.DescuentoManualValor, descuentoAplicado);
                }

                if (descuentoAplicado > subtotal)
                {
                    _logger.LogError("Discount exceeds subtotal. Descuento: {Descuento}, Subtotal: {Subtotal}", descuentoAplicado, subtotal);
                    throw new InvalidOperationException("El descuento no puede ser mayor al subtotal de la compra.");
                }

                decimal totalCalculado = subtotal - descuentoAplicado;

                // Validar efectivo
                bool esEfectivo = metodoPago.Nombre.ToLower().Contains("efectivo");
                decimal? vuelto = null;

                if (esEfectivo)
                {
                    if (!request.MontoRecibido.HasValue || request.MontoRecibido.Value <= 0)
                    {
                        _logger.LogWarning("Cash payment without amount received");
                        throw new InvalidOperationException("Para pago en efectivo debe ingresar el monto recibido.");
                    }
                    if (request.MontoRecibido.Value < totalCalculado)
                    {
                        _logger.LogWarning("Insufficient cash amount. Recibido: {Recibido}, Total: {Total}", request.MontoRecibido.Value, totalCalculado);
                        throw new InvalidOperationException($"El monto recibido (S/. {request.MontoRecibido.Value:F2}) es menor al total (S/. {totalCalculado:F2}).");
                    }
                    vuelto = Math.Round(request.MontoRecibido.Value - totalCalculado, 2);
                }

                // Crear venta
                var venta = new Venta
                {
                    ClienteId = request.ClienteId,
                    VendedorId = request.VendedorId,
                    MetodoPagoId = request.MetodoPagoId,
                    Fecha = DateTime.UtcNow,
                    Total = totalCalculado,
                    Descuento = descuentoAplicado,
                    DescuentoAutorizadoId = descuentoIdAplicado,
                    MontoRecibido = esEfectivo ? request.MontoRecibido : null,
                    Vuelto = vuelto
                };

                await _unitOfWork.Ventas.AddAsync(venta);
                await _unitOfWork.CommitAsync();

                // Insertar detalles y descontar stock
                foreach (var (det, prenda) in detallesConPrecio)
                {
                    await _unitOfWork.DetalleVentas.AddAsync(new DetalleVenta
                    {
                        VentaId = venta.Id,
                        PrendaId = det.PrendaId,
                        Cantidad = det.Cantidad,
                        Precio = prenda.Precio,
                        Subtotal = prenda.Precio * det.Cantidad
                    });

                    int stockAntes = prenda.Stock;
                    prenda.Stock -= det.Cantidad;
                    _unitOfWork.Prendas.Update(prenda);

                    _logger.LogDebug("Stock updated. Prenda: {Prenda}, StockAntes: {StockAntes}, StockDespues: {StockDespues}, Cantidad: {Cantidad}", 
                        prenda.Nombre, stockAntes, prenda.Stock, det.Cantidad);
                }

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                return venta.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during sale registration. Transaction rolled back");
                throw new Exception($"Error al registrar la venta: {ex.Message}", ex);
            }
        }

        private async Task<(bool exito, string mensaje)> ValidarVentaInterno(int clienteId, int vendedorId,
            int metodoPagoId, List<DetalleVentaDTO> detalles)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
            if (cliente == null) return (false, "Cliente no encontrado.");

            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(vendedorId);
            if (vendedor == null) return (false, "Vendedor no encontrado.");

            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(metodoPagoId);
            if (metodoPago == null) return (false, "Metodo de pago no encontrado.");

            if (detalles == null || !detalles.Any())
                return (false, "La venta debe contener al menos un producto.");

            foreach (var detalle in detalles)
            {
                var prenda = await _unitOfWork.Prendas.GetByIdAsync(detalle.PrendaId);
                if (prenda == null) return (false, $"Producto {detalle.PrendaId} no encontrado.");
                if (prenda.Stock < detalle.Cantidad)
                    return (false, $"Stock insuficiente para {prenda.Nombre}. Disponible: {prenda.Stock}, solicitado: {detalle.Cantidad}.");
            }

            return (true, "Validacion exitosa.");
        }
    }

    // =========================================
    // REQUEST MODELS
    // =========================================

    public class RegistrarVentaRequest
    {
        public int ClienteId { get; set; }
        public int VendedorId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal? MontoRecibido { get; set; }
        public int? DescuentoAutorizadoId { get; set; }
        public string TipoDescuento { get; set; } = "Ninguno";
        public decimal DescuentoManualValor { get; set; } = 0;
        public List<DetalleVentaRequest> Detalles { get; set; } = new();
    }

    public class DetalleVentaRequest
    {
        public int PrendaId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }

    public class ClienteRapidoRequest
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }
}

