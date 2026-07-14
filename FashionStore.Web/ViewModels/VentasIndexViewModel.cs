using FashionStore.Domain.Entities;

namespace FashionStore.Web.ViewModels
{
    public class VentasIndexViewModel
    {
        public List<Venta> Ventas { get; set; } = new();
        public int VentasHoy { get; set; }
        public decimal IngresosHoy { get; set; }
        public int TotalVentas { get; set; }
        public decimal IngresosTotal { get; set; }
    }
}
