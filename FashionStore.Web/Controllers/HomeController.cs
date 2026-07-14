using System.Diagnostics;
using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // _context se mantiene SOLO para: _context.Users (Identity) y consultas que requieren Include()
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;
        private readonly FashionStore.Domain.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            FashionStore.Infrastructure.Context.FashionStoreDbContext context,
            FashionStore.Domain.Interfaces.IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new FashionStore.Web.ViewModels.DashboardViewModel();

            // ============================================
            // TOTALES — usar IUnitOfWork para consultas simples
            // ============================================

            var allCategorias = await _unitOfWork.Categorias.GetAllAsync();
            var allPrendas    = await _unitOfWork.Prendas.GetAllAsync();
            var allClientes   = await _unitOfWork.Clientes.GetAllAsync();
            var allVentas     = await _unitOfWork.Ventas.GetAllAsync();

            vm.TotalCategorias = allCategorias.Count();
            vm.TotalPrendas    = allPrendas.Count();
            vm.TotalClientes   = allClientes.Count();
            vm.TotalVentas     = allVentas.Count();
            vm.TotalStock      = allPrendas.Sum(p => p.Stock);
            vm.TotalIngresos   = allVentas.Sum(v => v.Total);

            // TotalUsuarios requiere _context.Users (ASP.NET Identity — sin repositorio disponible)
            vm.TotalUsuarios = await _context.Users.CountAsync();

            // ============================================
            // GRÁFICO MENSUAL (últimos 6 meses)
            // ============================================

            var sixMonthsAgo = DateTime.Now.AddMonths(-5);
            var salesLastSixMonths = allVentas
                .Where(v => v.Fecha >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Total = g.Sum(x => x.Total)
                })
                .ToList();

            vm.SalesChartLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                salesLastSixMonths.Select(s => s.Label).ToArray());
            vm.SalesChartDataJson = System.Text.Json.JsonSerializer.Serialize(
                salesLastSixMonths.Select(s => s.Total).ToArray());

            // ============================================
            // GRÁFICO SEMANAL (últimos 7 días)
            // ============================================

            var sevenDaysAgo = DateTime.Now.AddDays(-6);
            var salesLastSevenDays = allVentas
                .Where(v => v.Fecha.Date >= sevenDaysAgo.Date)
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Label = g.Key.ToString("ddd dd/MMM"),
                    Total = g.Sum(x => x.Total)
                })
                .ToList();

            vm.WeeklySalesLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                salesLastSevenDays.Select(s => s.Label).ToArray());
            vm.WeeklySalesDataJson = System.Text.Json.JsonSerializer.Serialize(
                salesLastSevenDays.Select(s => s.Total).ToArray());

            // ============================================
            // PRENDAS POR CATEGORÍA — requiere Include, se mantiene _context
            // ============================================

            var prendasWithCat = await _context.Set<Prenda>()
                .Include(p => p.Categoria)
                .ToListAsync();

            var groupByCat = prendasWithCat
                .GroupBy(p => p.Categoria?.Nombre ?? "Sin categor\u00eda")
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            vm.PrendasByCategoryLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                groupByCat.Select(x => x.Name).ToArray());
            vm.PrendasByCategoryDataJson = System.Text.Json.JsonSerializer.Serialize(
                groupByCat.Select(x => x.Count).ToArray());

            // ============================================
            // DETALLES PARA TOP PRODUCTOS — requiere Include
            // ============================================

            var allDetalles = await _context.Set<DetalleVenta>()
                .Include(d => d.Prenda)
                .ToListAsync();

            var topProducts = allDetalles
                .GroupBy(d => new { d.Prenda?.Id, d.Prenda?.Nombre })
                .Select(g => new
                {
                    Id = g.Key.Id,
                    Name = g.Key.Nombre ?? "Desconocido",
                    Qty = g.Sum(x => x.Cantidad),
                    Revenue = g.Sum(x => x.Precio * x.Cantidad)
                })
                .OrderByDescending(x => x.Qty)
                .Take(10)
                .ToList();

            vm.TopProductsLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                topProducts.Select(x => x.Name).ToArray());
            vm.TopProductsDataJson = System.Text.Json.JsonSerializer.Serialize(
                topProducts.Select(x => x.Qty).ToArray());

            vm.TopSellingProducts = topProducts.Select(x => new
            {
                x.Name,
                x.Qty,
                x.Revenue
            }).ToList();

            // ============================================
            // INGRESOS POR MÉTODO DE PAGO — requiere Include
            // ============================================

            var ventasWithMethod = await _context.Set<Venta>()
                .Include(v => v.MetodoPago)
                .ToListAsync();

            var revenueByMethod = ventasWithMethod
                .GroupBy(v => v.MetodoPago?.Nombre ?? "Desconocido")
                .Select(g => new
                {
                    Method = g.Key,
                    Revenue = g.Sum(x => x.Total)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            vm.RevenueByMethodLabelsJson = System.Text.Json.JsonSerializer.Serialize(
                revenueByMethod.Select(x => x.Method).ToArray());
            vm.RevenueByMethodDataJson = System.Text.Json.JsonSerializer.Serialize(
                revenueByMethod.Select(x => x.Revenue).ToArray());

            // ============================================
            // PRODUCTOS AGOTÁNDOSE (stock ≤ 5) — usa IUnitOfWork + FindAsync
            // ============================================

            var prendasAgotandose = await _context.Set<Prenda>()
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0 && p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToListAsync();

            var prendasAgotadas = await _context.Set<Prenda>()
                .Include(p => p.Categoria)
                .Where(p => p.Stock == 0)
                .OrderBy(p => p.Nombre)
                .Take(10)
                .ToListAsync();

            var totalPrendasStockNormal = (int)allPrendas.Count(p => p.Stock > 5);

            vm.PrendasAgotandose = _mapper.Map<List<PrendaDTO>>(prendasAgotandose);
            vm.PrendasAgotadas = _mapper.Map<List<PrendaDTO>>(prendasAgotadas);
            vm.CantidadAgotadas = (int)allPrendas.Count(p => p.Stock == 0);
            vm.CantidadBajoStock = (int)allPrendas.Count(p => p.Stock > 0 && p.Stock <= 5);
            vm.CantidadStockNormal = totalPrendasStockNormal;

            vm.LowStockProducts = prendasAgotandose
                .Select(p => new { p.Nombre, p.Stock, p.Precio })
                .ToList();

            // ============================================
            // PRENDAS DESTACADAS (últimas 6) — requiere Include + OrderBy
            // ============================================

            var prendasDestacadas = await _context.Set<Prenda>()
                .Include(p => p.Categoria)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            vm.Prendas = _mapper.Map<List<PrendaDTO>>(prendasDestacadas);

            // ============================================
            // ÚLTIMAS VENTAS (últimas 10) — usa IUnitOfWork
            // ============================================

            vm.RecentSales = allVentas
                .OrderByDescending(v => v.Fecha)
                .Take(10)
                .Select(v => new
                {
                    v.Id,
                    v.Fecha,
                    v.Total,
                    MetodoPago = v.MetodoPago?.Nombre ?? "Desconocido",
                    Cliente = v.Cliente?.NombreCompleto ?? "Cliente"
                })
                .ToList();

            // ============================================
            // CLIENTES RECIENTES — usa IUnitOfWork
            // ============================================

            vm.RecentClients = allClientes
                .OrderByDescending(c => c.Id)
                .Take(8)
                .Select(c => new
                {
                    c.NombreCompleto,
                    c.DNI,
                    c.Telefono
                })
                .ToList();

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
