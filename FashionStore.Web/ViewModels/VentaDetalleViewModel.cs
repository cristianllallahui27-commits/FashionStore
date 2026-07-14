namespace FashionStore.Web.ViewModels
{
    public class DetalleVentaItem
    {
        public string NombrePrenda { get; set; } = string.Empty;
        public string CodigoBarra { get; set; } = string.Empty;
        public string Talla { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class VentaDetalleViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        // Cliente
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteDNI { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;

        // Vendedor
        public string VendedorNombre { get; set; } = string.Empty;

        // Pago
        public string MetodoPagoNombre { get; set; } = string.Empty;
        public bool EsEfectivo => MetodoPagoNombre.ToLower().Contains("efectivo");
        public decimal? MontoRecibido { get; set; }
        public decimal? Vuelto { get; set; }
        public decimal Descuento { get; set; } = 0;
        public string? DescuentoNombre { get; set; }

        // Lineas de detalle
        public List<DetalleVentaItem> Detalles { get; set; } = new();

        // Totales calculados
        public decimal Subtotal => Detalles.Sum(d => d.Subtotal);
        public decimal Total { get; set; }

        // Branding tienda
        public string NombreTienda { get; set; } = "FashionStore";
        public string? TelefonoTienda { get; set; }
        public string? CorreoTienda { get; set; }
        public string? DireccionTienda { get; set; }
        public string? RutaLogo { get; set; }
    }
}
