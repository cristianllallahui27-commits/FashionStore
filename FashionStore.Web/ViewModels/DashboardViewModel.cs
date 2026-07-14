using FashionStore.Domain.DTOs;
using System.Collections.Generic;

namespace FashionStore.Web.ViewModels
{
    public class DashboardViewModel
    {
        // Totales
        public decimal TotalIngresos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalPrendas { get; set; }
        public int TotalClientes { get; set; }
        public int TotalVentas { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalStock { get; set; }

        // Chart data serialized as JSON
        public string SalesChartLabelsJson { get; set; } = string.Empty;
        public string SalesChartDataJson { get; set; } = string.Empty;

        public string PrendasByCategoryLabelsJson { get; set; } = string.Empty;
        public string PrendasByCategoryDataJson { get; set; } = string.Empty;

        public string TopProductsLabelsJson { get; set; } = string.Empty;
        public string TopProductsDataJson { get; set; } = string.Empty;

        // Weekly sales chart
        public string WeeklySalesLabelsJson { get; set; } = string.Empty;
        public string WeeklySalesDataJson { get; set; } = string.Empty;

        // Recent clients
        public IEnumerable<object>? RecentClients { get; set; }

        // Low stock products — alias PrendasAgotandose para la vista Home/Index
        public IList<PrendaDTO>? PrendasAgotandose { get; set; }

        // Mantiene LowStockProducts para compatibilidad con otras vistas
        public IEnumerable<object>? LowStockProducts { get; set; }

        // Stock summary counts for Home widget
        public int CantidadAgotadas { get; set; }
        public int CantidadBajoStock { get; set; }
        public int CantidadStockNormal { get; set; }
        public IList<PrendaDTO>? PrendasAgotadas { get; set; }

        // Prendas destacadas para la vista Home/Index
        public IList<PrendaDTO>? Prendas { get; set; }

        // Recent sales
        public IEnumerable<object>? RecentSales { get; set; }

        // Top selling products
        public IEnumerable<object>? TopSellingProducts { get; set; }

        // Revenue by payment method
        public string RevenueByMethodLabelsJson { get; set; } = string.Empty;
        public string RevenueByMethodDataJson { get; set; } = string.Empty;
    }
}
