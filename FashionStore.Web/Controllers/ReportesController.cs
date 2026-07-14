using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly FashionStoreDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            FashionStoreDbContext context,
            IUnitOfWork unitOfWork,
            ILogger<ReportesController> logger)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            try
            {
                var vendedores = await _unitOfWork.Vendedores.FindAsync(v => v.Estado);
                var ventas = await _context.Ventas
                    .Include(v => v.Vendedor)
                    .Include(v => v.DetalleVentas)
                    .ToListAsync();

                var reporteSummary = vendedores.Select(v => new
                {
                    VendedorId = v.Id,
                    Vendedor = $"{v.Nombres} {v.Apellidos}",
                    TotalVentas = ventas.Where(vta => vta.VendedorId == v.Id).Count(),
                    MontoTotal = ventas.Where(vta => vta.VendedorId == v.Id).Sum(vta => vta.Total),
                    ProductosTotales = ventas.Where(vta => vta.VendedorId == v.Id)
                        .SelectMany(vta => vta.DetalleVentas ?? new List<DetalleVenta>())
                        .Sum(dv => dv.Cantidad)
                }).OrderByDescending(r => r.MontoTotal).ToList();

                ViewBag.ReporteSummary = reporteSummary;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar página de reportes");
                TempData["Error"] = "Error al cargar los reportes.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Reportes/DetalleVendedor/5
        public async Task<IActionResult> DetalleVendedor(int id)
        {
            try
            {
                var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
                if (vendedor == null) return NotFound();

                var ventas = await _context.Ventas
                    .Where(v => v.VendedorId == id)
                    .Include(v => v.Cliente)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Prenda)
                    .OrderByDescending(v => v.Fecha)
                    .ToListAsync();

                ViewBag.Vendedor = vendedor;
                return View(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de vendedor");
                TempData["Error"] = "Error al cargar el detalle.";
                return RedirectToAction("Index");
            }
        }

        // GET: Reportes/ExcelVendedores (genera Excel con todas las ventas por vendedor)
        [HttpGet("reportes/excel-vendedores")]
        public async Task<IActionResult> ExcelVendedores()
        {
            try
            {
                var vendedores = await _unitOfWork.Vendedores.FindAsync(v => v.Estado);
                var ventas = await _context.Ventas
                    .Include(v => v.Vendedor)
                    .Include(v => v.Cliente)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Prenda)
                    .ToListAsync();

                // Generar contenido CSV (Excel puede abrirlo)
                var csv = new StringBuilder();
                csv.AppendLine("REPORTE DE VENTAS POR VENDEDOR");
                csv.AppendLine($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                csv.AppendLine();

                // Resumen por vendedor
                csv.AppendLine("RESUMEN POR VENDEDOR");
                csv.AppendLine("Vendedor,Total Ventas,Monto Total,Productos Vendidos");
                
                foreach (var vendedor in vendedores.OrderByDescending(v => 
                    ventas.Where(vt => vt.VendedorId == v.Id).Sum(vt => vt.Total)))
                {
                    var ventasVendedor = ventas.Where(v => v.VendedorId == vendedor.Id);
                    var totalVentas = ventasVendedor.Count();
                    var montoTotal = ventasVendedor.Sum(v => v.Total);
                    var productosVendidos = ventasVendedor.SelectMany(v => v.DetalleVentas ?? new List<DetalleVenta>())
                        .Sum(dv => dv.Cantidad);

                    csv.AppendLine($"\"{vendedor.Nombres} {vendedor.Apellidos}\",{totalVentas},S/. {montoTotal:F2},{productosVendidos}");
                }

                csv.AppendLine();
                csv.AppendLine("DETALLE DE VENTAS");
                csv.AppendLine("Vendedor,Fecha,Cliente,Método Pago,Producto,Cantidad,Precio Unitario,Subtotal");

                foreach (var venta in ventas.OrderBy(v => v.Vendedor!.Nombres).ThenByDescending(v => v.Fecha))
                {
                    foreach (var detalle in venta.DetalleVentas ?? new List<DetalleVenta>())
                    {
                        csv.AppendLine($"\"{venta.Vendedor!.Nombres} {venta.Vendedor.Apellidos}\"," +
                            $"{venta.Fecha:dd/MM/yyyy}," +
                            $"\"{venta.Cliente!.NombreCompleto}\"," +
                            $"\"{venta.MetodoPago!.Nombre}\"," +
                            $"\"{detalle.Prenda!.Nombre}\"," +
                            $"{detalle.Cantidad}," +
                            $"S/. {detalle.Precio:F2}," +
                            $"S/. {detalle.Subtotal:F2}");
                    }
                }

                var fileName = $"Reporte_Vendedores_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar Excel");
                TempData["Error"] = "Error al generar el archivo Excel.";
                return RedirectToAction("Index");
            }
        }

        // GET: Reportes/PdfVendedor/5 (genera PDF con detalles de un vendedor)
        [HttpGet("reportes/pdf-vendedor/{id}")]
        public async Task<IActionResult> PdfVendedor(int id)
        {
            try
            {
                var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
                if (vendedor == null) return NotFound();

                var ventas = await _context.Ventas
                    .Where(v => v.VendedorId == id)
                    .Include(v => v.Cliente)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.DetalleVentas)
                    .ThenInclude(dv => dv.Prenda)
                    .OrderByDescending(v => v.Fecha)
                    .ToListAsync();

                // Generar HTML para PDF (usando iTextSharp u otra librería)
                var html = GenerarHtmlPdfVendedor(vendedor, ventas);

                // Por ahora, devolver HTML que puede imprimirse
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF");
                TempData["Error"] = "Error al generar el PDF.";
                return RedirectToAction("Index");
            }
        }

        private string GenerarHtmlPdfVendedor(Vendedor vendedor, List<Venta> ventas)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("<title>Reporte Vendedor</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { color: #0d47a1; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #0d47a1; color: white; }");
            html.AppendLine(".totales { margin-top: 20px; font-weight: bold; }");
            html.AppendLine(".print-btn { margin-top: 20px; padding: 10px 20px; background-color: #198754; color: white; border: none; cursor: pointer; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine($"<h1>Reporte de Ventas - {vendedor.Nombres} {vendedor.Apellidos}</h1>");
            html.AppendLine($"<p><strong>Fecha de Generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
            html.AppendLine($"<p><strong>DNI:</strong> {vendedor.DNI}</p>");
            html.AppendLine($"<p><strong>Correo:</strong> {vendedor.Correo}</p>");

            html.AppendLine("<table>");
            html.AppendLine("<thead>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>Fecha</th>");
            html.AppendLine("<th>Cliente</th>");
            html.AppendLine("<th>Producto</th>");
            html.AppendLine("<th>Cantidad</th>");
            html.AppendLine("<th>Precio Unitario</th>");
            html.AppendLine("<th>Subtotal</th>");
            html.AppendLine("<th>Método Pago</th>");
            html.AppendLine("</tr>");
            html.AppendLine("</thead>");
            html.AppendLine("<tbody>");

            decimal totalGeneral = 0;
            int productosTotales = 0;

            foreach (var venta in ventas)
            {
                foreach (var detalle in venta.DetalleVentas ?? new List<DetalleVenta>())
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{venta.Fecha:dd/MM/yyyy}</td>");
                    html.AppendLine($"<td>{venta.Cliente?.NombreCompleto}</td>");
                    html.AppendLine($"<td>{detalle.Prenda?.Nombre}</td>");
                    html.AppendLine($"<td>{detalle.Cantidad}</td>");
                    html.AppendLine($"<td>S/. {detalle.Precio:F2}</td>");
                    html.AppendLine($"<td>S/. {detalle.Subtotal:F2}</td>");
                    html.AppendLine($"<td>{venta.MetodoPago?.Nombre}</td>");
                    html.AppendLine("</tr>");

                    totalGeneral += detalle.Subtotal;
                    productosTotales += detalle.Cantidad;
                }
            }

            html.AppendLine("</tbody>");
            html.AppendLine("</table>");

            html.AppendLine("<div class='totales'>");
            html.AppendLine($"<p>Total de Ventas: {ventas.Count}</p>");
            html.AppendLine($"<p>Total de Productos Vendidos: {productosTotales}</p>");
            html.AppendLine($"<p>Monto Total: S/. {totalGeneral:F2}</p>");
            html.AppendLine("</div>");

            html.AppendLine("<button class='print-btn' onclick='window.print();'>Imprimir / Descargar PDF</button>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}
