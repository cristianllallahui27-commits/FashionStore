using Microsoft.AspNetCore.Mvc.Rendering;

namespace FashionStore.Web.ViewModels
{
    public class VentaCreateViewModel
    {
        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> MetodosPago { get; set; } = new();
        public List<SelectListItem> DescuentosActivos { get; set; } = new();
        public string DescuentosJson { get; set; } = "[]";
        public int? VendedorPreseleccionadoId { get; set; }
        
        // Vendedor automático desde sesión
        public int? VendedorLogueadoId { get; set; }
        public string VendedorLogueadoNombre { get; set; } = "";
        public bool VendedorAutoDetectado { get; set; } = false;
    }
}
