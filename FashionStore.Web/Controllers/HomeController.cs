using System.Diagnostics;
using FashionStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace FashionStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;
        private readonly FashionStore.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, FashionStore.Infrastructure.Context.FashionStoreDbContext context, FashionStore.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new FashionStore.Web.ViewModels.DashboardViewModel();

            // Totals
            vm.TotalCategorias = (await _unitOfWork.Categorias.GetAllAsync()).Count();
            vm.TotalPrendas = (await _unitOfWork.Prendas.GetAllAsync()).Count();
            vm.TotalClientes = (await _unitOfWork.Clientes.GetAllAsync()).Count();
            vm.TotalVentas = (await _unitOfWork.Ventas.GetAllAsync()).Count();
            vm.TotalUsuarios = await _context.Users.CountAsync();

            // Sales per month - last 6 months
            var sixMonthsAgo = DateTime.Now.AddMonths(-5);
            var sales = await _context.Set<FashionStore.Domain.Entities.Venta>()
                .Where(v => v.Fecha >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .ToListAsync();

            var salesGrouped = sales
                .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"), Total = g.Sum(x => x.Total) })
                .ToList();

            var labels = salesGrouped.Select(s => s.Label).ToArray();
            var data = salesGrouped.Select(s => s.Total).ToArray();

            vm.SalesChartLabelsJson = System.Text.Json.JsonSerializer.Serialize(labels);
            vm.SalesChartDataJson = System.Text.Json.JsonSerializer.Serialize(data);

            // Prendas by category
            var prendas = await _context.Set<FashionStore.Domain.Entities.Prenda>().Include(p => p.Categoria).ToListAsync();
            var groupByCat = prendas.GroupBy(p => p.Categoria?.Nombre ?? "Sin categoría")
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count).ToList();

            vm.PrendasByCategoryLabelsJson = System.Text.Json.JsonSerializer.Serialize(groupByCat.Select(x => x.Name).ToArray());
            vm.PrendasByCategoryDataJson = System.Text.Json.JsonSerializer.Serialize(groupByCat.Select(x => x.Count).ToArray());

            // Top products (by total sold quantity)
            var detalles = await _context.Set<FashionStore.Domain.Entities.DetalleVenta>().Include(d => d.Prenda).ToListAsync();
            var topProducts = detalles.GroupBy(d => d.Prenda?.Nombre ?? "Desconocido")
                .Select(g => new { Name = g.Key, Qty = g.Sum(x => x.Cantidad) })
                .OrderByDescending(x => x.Qty).Take(5).ToList();

            vm.TopProductsLabelsJson = System.Text.Json.JsonSerializer.Serialize(topProducts.Select(x => x.Name).ToArray());
            vm.TopProductsDataJson = System.Text.Json.JsonSerializer.Serialize(topProducts.Select(x => x.Qty).ToArray());

            // Recent clients
            var recentClients = (await _unitOfWork.Clientes.GetAllAsync())
                .OrderByDescending(c => c.Id).Take(5).Select(c => new { c.NombreCompleto, c.DNI, c.Telefono }).ToList();

            vm.RecentClients = recentClients;

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
