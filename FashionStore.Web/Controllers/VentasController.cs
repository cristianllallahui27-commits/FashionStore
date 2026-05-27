using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Web.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Web.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;

        private readonly IUnitOfWork _unitOfWork;

        public VentasController(
            FashionStore.Infrastructure.Context.FashionStoreDbContext context,
            IUnitOfWork unitOfWork)
        {
            _context = context;

            _unitOfWork = unitOfWork;
        }

        // =========================================
        // INDEX
        // =========================================

        public async Task<IActionResult> Index()
        {
            var ventas =
                await _unitOfWork.Ventas.GetAllAsync();

            return View(ventas);
        }

        // =========================================
        // DASHBOARD MODERNO
        // =========================================

        public async Task<IActionResult> Dashboard()
        {
            // =====================================
            // VENTAS
            // =====================================

            var ventas = await _context
                .Set<Venta>()
                .Include(v => v.Cliente)
                .ToListAsync();

            // =====================================
            // TOTALES
            // =====================================

            var totalVentas =
                ventas.Count();

            var totalIngresos =
                ventas.Sum(v => v.Total);

            // =====================================
            // VENTAS POR MES
            // =====================================

            var ventasPorMes = ventas

                .GroupBy(v => new
                {
                    v.Fecha.Year,
                    v.Fecha.Month
                })

                .Select(g => new
                {
                    Label =
                        new DateTime(
                            g.Key.Year,
                            g.Key.Month,
                            1).ToString("MMM yyyy"),

                    Total =
                        g.Count(),

                    Ingresos =
                        g.Sum(x => x.Total)
                })

                .OrderBy(x => x.Label)

                .ToList();

            // =====================================
            // LABELS Y DATA
            // =====================================

            var labels = ventasPorMes
                .Select(x => x.Label)
                .ToArray();

            var totals = ventasPorMes
                .Select(x => x.Total)
                .ToArray();

            var ingresos = ventasPorMes
                .Select(x => x.Ingresos)
                .ToArray();

            // =====================================
            // DETALLE VENTAS
            // =====================================

            var detalles = await _context
                .Set<DetalleVenta>()

                .Include(d => d.Prenda)

                .ThenInclude(p => p.Categoria)

                .ToListAsync();

            // =====================================
            // VENTAS POR CATEGORÍA
            // =====================================

            var ventasPorCategoria = detalles

                .GroupBy(d =>
                    d.Prenda?.Categoria?.Nombre
                    ?? "Sin categoría")

                .Select(g => new
                {
                    Categoria = g.Key,

                    Total = g.Sum(x => x.Subtotal)
                })

                .OrderByDescending(x => x.Total)

                .ToList();

            // =====================================
            // VIEWMODEL
            // =====================================

            var model = new DashboardViewModel
            {
                TotalVentas = totalVentas,

                TotalClientes =
                    ventas
                    .Select(v => v.ClienteId)
                    .Distinct()
                    .Count(),

                TotalIngresos =
                    totalIngresos,

                SalesChartLabelsJson =
                    System.Text.Json.JsonSerializer.Serialize(labels),

                SalesChartDataJson =
                    System.Text.Json.JsonSerializer.Serialize(ingresos),

                PrendasByCategoryLabelsJson =
                    System.Text.Json.JsonSerializer.Serialize(
                        ventasPorCategoria
                        .Select(x => x.Categoria)
                        .ToArray()),

                PrendasByCategoryDataJson =
                    System.Text.Json.JsonSerializer.Serialize(
                        ventasPorCategoria
                        .Select(x => x.Total)
                        .ToArray())
            };

            return View(model);
        }
    }
}