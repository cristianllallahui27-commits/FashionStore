using System.Collections.Generic;

namespace FashionStore.Web.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalIngresos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalPrendas { get; set; }
        public int TotalClientes { get; set; }
        public int TotalVentas { get; set; }
        public int TotalUsuarios { get; set; }

        // Chart data serialized as JSON
        public string SalesChartLabelsJson { get; set; } = string.Empty;
        public string SalesChartDataJson { get; set; } = string.Empty;

        public string PrendasByCategoryLabelsJson { get; set; } = string.Empty;
        public string PrendasByCategoryDataJson { get; set; } = string.Empty;

        public string TopProductsLabelsJson { get; set; } = string.Empty;
        public string TopProductsDataJson { get; set; } = string.Empty;

        public IEnumerable<object>? RecentClients { get; set; }
    }
}
