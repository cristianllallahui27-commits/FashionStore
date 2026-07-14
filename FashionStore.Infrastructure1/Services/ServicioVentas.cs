using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;

namespace FashionStore.Infrastructure.Services
{
    public class ServicioVentas : IServicioVentas
    {
        private readonly FashionStoreDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ServicioVentas(FashionStoreDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // REGISTRAR VENTA
        // Toda la operación es atómica: una sola transacción,
        // un solo SaveChangesAsync al final.
        // ============================================================
        public async Task<int> RegistrarVenta(int clienteId, int vendedorId, int metodoPagoId,
            List<DetalleVentaDTO> detalles, string? usuarioAutenticadoId = null)
        {
            // Validar antes de abrir la transacción
            var (exito, mensaje) = await ValidarVenta(clienteId, vendedorId, metodoPagoId, detalles);
            if (!exito)
                throw new InvalidOperationException(mensaje);

            // Una sola transacción para venta + detalles + stock
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear la venta
                var venta = new Venta
                {
                    ClienteId    = clienteId,
                    VendedorId   = vendedorId,
                    MetodoPagoId = metodoPagoId,
                    Fecha        = DateTime.Now,
                    Total        = await CalcularTotalVenta(detalles)
                };

                await _unitOfWork.Ventas.AddAsync(venta);
                await _unitOfWork.CommitAsync();  // necesario para obtener venta.Id

                // 2. Agregar detalles y actualizar stock en memoria
                foreach (var detalle in detalles)
                {
                    var detalleVenta = new DetalleVenta
                    {
                        VentaId  = venta.Id,
                        PrendaId = detalle.PrendaId,
                        Cantidad = detalle.Cantidad,
                        Precio   = detalle.Precio,
                        Subtotal = detalle.Precio * detalle.Cantidad
                    };

                    await _unitOfWork.DetalleVentas.AddAsync(detalleVenta);

                    // Modifica el stock en memoria (sin SaveChangesAsync propio)
                    await ActualizarInventario(detalle.PrendaId, detalle.Cantidad);
                }

                // 3. Un único commit para detalles + stock
                await _unitOfWork.CommitAsync();

                // 4. Confirmar la transacción completa
                await transaction.CommitAsync();

                return venta.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al registrar la venta: {ex.Message}", ex);
            }
        }

        // ============================================================
        // ACTUALIZAR INVENTARIO
        // Solo modifica la entidad en memoria — NO llama SaveChangesAsync.
        // El commit lo realiza RegistrarVenta() en un único punto.
        // También se puede llamar de forma standalone: en ese caso el
        // llamador es responsable de hacer CommitAsync().
        // ============================================================
        public async Task<bool> ActualizarInventario(int prendaId, int cantidad)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(prendaId);
            if (prenda == null)
                return false;

            if (prenda.Stock < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente para la prenda Id={prendaId}. " +
                    $"Disponible: {prenda.Stock}, Solicitado: {cantidad}");

            prenda.Stock -= cantidad;
            _unitOfWork.Prendas.Update(prenda);

            // Sin SaveChangesAsync — el commit queda en manos del llamador
            return true;
        }

        // ============================================================
        // CALCULAR TOTAL
        // ============================================================
        public Task<decimal> CalcularTotalVenta(List<DetalleVentaDTO> detalles)
        {
            var total = detalles.Sum(d => d.Precio * d.Cantidad);
            return Task.FromResult(total);
        }

        // ============================================================
        // VALIDAR VENTA
        // Usa IUnitOfWork para consultas simples — sin acceso directo a _context.
        // ============================================================
        public async Task<(bool exito, string mensaje)> ValidarVenta(int clienteId, int vendedorId,
            int metodoPagoId, List<DetalleVentaDTO> detalles)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
            if (cliente == null)
                return (false, "Cliente no encontrado");

            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(vendedorId);
            if (vendedor == null)
                return (false, "Vendedor no encontrado");

            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(metodoPagoId);
            if (metodoPago == null)
                return (false, "Método de pago no encontrado");

            if (detalles == null || !detalles.Any())
                return (false, "La venta debe contener al menos un producto");

            foreach (var detalle in detalles)
            {
                var prenda = await _unitOfWork.Prendas.GetByIdAsync(detalle.PrendaId);
                if (prenda == null)
                    return (false, $"Producto {detalle.PrendaId} no encontrado");

                if (prenda.Stock < detalle.Cantidad)
                    return (false,
                        $"Stock insuficiente para {prenda.Nombre}. " +
                        $"Disponible: {prenda.Stock}, Solicitado: {detalle.Cantidad}");
            }

            return (true, "Validación exitosa");
        }
    }
}
