using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Tests.Repositories
{
    [TestClass]
    public class GenericRepositoryExtendedTests
    {
        private FashionStoreDbContext _context = null!;
        private GenericRepository<Venta> _ventaRepository = null!;
        private GenericRepository<DetalleVenta> _detalleVentaRepository = null!;
        private GenericRepository<MetodoPago> _metodoPagoRepository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FashionStoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"FashionStoreDb_{Guid.NewGuid()}")
                .Options;

            _context = new FashionStoreDbContext(options);
            _ventaRepository = new GenericRepository<Venta>(_context);
            _detalleVentaRepository = new GenericRepository<DetalleVenta>(_context);
            _metodoPagoRepository = new GenericRepository<MetodoPago>(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region Venta Repository Tests

        [TestMethod]
        public async Task VentaRepository_GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _ventaRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task VentaRepository_AddAsync_WithValidVenta_AddsToDatabase()
        {
            // Arrange
            var venta = new Venta { ClienteId = 1, VendedorId = 1, MetodoPagoId = 1, Total = 100m };

            // Act
            await _ventaRepository.AddAsync(venta);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _ventaRepository.GetAllAsync();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(100m, result.First().Total);
        }

        [TestMethod]
        public async Task VentaRepository_GetByIdAsync_WithValidId_ReturnsVenta()
        {
            // Arrange
            var venta = new Venta { ClienteId = 1, VendedorId = 1, MetodoPagoId = 1, Total = 50m };
            await _ventaRepository.AddAsync(venta);
            await _context.SaveChangesAsync();

            var addedVenta = _context.Set<Venta>().First();

            // Act
            var result = await _ventaRepository.GetByIdAsync(addedVenta.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(50m, result.Total);
        }

        [TestMethod]
        public async Task VentaRepository_Update_WithValidVenta_UpdatesDatabase()
        {
            // Arrange
            var venta = new Venta { ClienteId = 1, VendedorId = 1, MetodoPagoId = 1, Total = 75m };
            await _ventaRepository.AddAsync(venta);
            await _context.SaveChangesAsync();

            var addedVenta = _context.Set<Venta>().First();
            addedVenta.Total = 150m;

            // Act
            _ventaRepository.Update(addedVenta);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _ventaRepository.GetByIdAsync(addedVenta.Id);
            Assert.AreEqual(150m, result?.Total);
        }

        [TestMethod]
        public async Task VentaRepository_Delete_WithValidVenta_RemovesFromDatabase()
        {
            // Arrange
            var venta = new Venta { ClienteId = 1, VendedorId = 1, MetodoPagoId = 1, Total = 100m };
            await _ventaRepository.AddAsync(venta);
            await _context.SaveChangesAsync();

            var addedVenta = _context.Set<Venta>().First();

            // Act
            _ventaRepository.Delete(addedVenta);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _ventaRepository.GetByIdAsync(addedVenta.Id);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task VentaRepository_FindAsync_WithValidExpression_ReturnsMatchingVentas()
        {
            // Arrange
            var venta1 = new Venta { ClienteId = 1, VendedorId = 1, MetodoPagoId = 1, Total = 100m };
            var venta2 = new Venta { ClienteId = 2, VendedorId = 1, MetodoPagoId = 1, Total = 200m };
            var venta3 = new Venta { ClienteId = 1, VendedorId = 2, MetodoPagoId = 1, Total = 150m };

            await _ventaRepository.AddAsync(venta1);
            await _ventaRepository.AddAsync(venta2);
            await _ventaRepository.AddAsync(venta3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ventaRepository.FindAsync(v => v.ClienteId == 1);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        #endregion

        #region DetalleVenta Repository Tests

        [TestMethod]
        public async Task DetalleVentaRepository_AddAsync_WithValidDetalle_AddsToDatabase()
        {
            // Arrange
            var detalle = new DetalleVenta { VentaId = 1, PrendaId = 1, Cantidad = 5, Precio = 29.99m, Subtotal = 149.95m };

            // Act
            await _detalleVentaRepository.AddAsync(detalle);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _detalleVentaRepository.GetAllAsync();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(5, result.First().Cantidad);
        }

        [TestMethod]
        public async Task DetalleVentaRepository_GetByIdAsync_WithValidId_ReturnsDetalle()
        {
            // Arrange
            var detalle = new DetalleVenta { VentaId = 1, PrendaId = 1, Cantidad = 3, Precio = 19.99m, Subtotal = 59.97m };
            await _detalleVentaRepository.AddAsync(detalle);
            await _context.SaveChangesAsync();

            var addedDetalle = _context.Set<DetalleVenta>().First();

            // Act
            var result = await _detalleVentaRepository.GetByIdAsync(addedDetalle.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Cantidad);
        }

        [TestMethod]
        public async Task DetalleVentaRepository_Update_WithValidDetalle_UpdatesDatabase()
        {
            // Arrange
            var detalle = new DetalleVenta { VentaId = 1, PrendaId = 1, Cantidad = 2, Precio = 10m, Subtotal = 20m };
            await _detalleVentaRepository.AddAsync(detalle);
            await _context.SaveChangesAsync();

            var addedDetalle = _context.Set<DetalleVenta>().First();
            addedDetalle.Cantidad = 10;

            // Act
            _detalleVentaRepository.Update(addedDetalle);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _detalleVentaRepository.GetByIdAsync(addedDetalle.Id);
            Assert.AreEqual(10, result?.Cantidad);
        }

        [TestMethod]
        public async Task DetalleVentaRepository_Delete_WithValidDetalle_RemovesFromDatabase()
        {
            // Arrange
            var detalle = new DetalleVenta { VentaId = 1, PrendaId = 1, Cantidad = 1, Precio = 5m, Subtotal = 5m };
            await _detalleVentaRepository.AddAsync(detalle);
            await _context.SaveChangesAsync();

            var addedDetalle = _context.Set<DetalleVenta>().First();

            // Act
            _detalleVentaRepository.Delete(addedDetalle);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _detalleVentaRepository.GetByIdAsync(addedDetalle.Id);
            Assert.IsNull(result);
        }

        #endregion

        #region MetodoPago Repository Tests

        [TestMethod]
        public async Task MetodoPagoRepository_AddAsync_WithValidMetodo_AddsToDatabase()
        {
            // Arrange
            var metodo = new MetodoPago { Nombre = "Tarjeta Crédito" };

            // Act
            await _metodoPagoRepository.AddAsync(metodo);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _metodoPagoRepository.GetAllAsync();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Tarjeta Crédito", result.First().Nombre);
        }

        [TestMethod]
        public async Task MetodoPagoRepository_GetByIdAsync_WithValidId_ReturnsMetodo()
        {
            // Arrange
            var metodo = new MetodoPago { Nombre = "Efectivo" };
            await _metodoPagoRepository.AddAsync(metodo);
            await _context.SaveChangesAsync();

            var addedMetodo = _context.Set<MetodoPago>().First();

            // Act
            var result = await _metodoPagoRepository.GetByIdAsync(addedMetodo.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Efectivo", result.Nombre);
        }

        [TestMethod]
        public async Task MetodoPagoRepository_GetAllAsync_WithMultipleMetodos_ReturnsAll()
        {
            // Arrange
            var metodo1 = new MetodoPago { Nombre = "Efectivo" };
            var metodo2 = new MetodoPago { Nombre = "Tarjeta" };
            var metodo3 = new MetodoPago { Nombre = "Cheque" };

            await _metodoPagoRepository.AddAsync(metodo1);
            await _metodoPagoRepository.AddAsync(metodo2);
            await _metodoPagoRepository.AddAsync(metodo3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _metodoPagoRepository.GetAllAsync();

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task MetodoPagoRepository_Update_WithValidMetodo_UpdatesDatabase()
        {
            // Arrange
            var metodo = new MetodoPago { Nombre = "Transferencia" };
            await _metodoPagoRepository.AddAsync(metodo);
            await _context.SaveChangesAsync();

            var addedMetodo = _context.Set<MetodoPago>().First();
            addedMetodo.Nombre = "Transferencia Bancaria";

            // Act
            _metodoPagoRepository.Update(addedMetodo);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _metodoPagoRepository.GetByIdAsync(addedMetodo.Id);
            Assert.AreEqual("Transferencia Bancaria", result?.Nombre);
        }

        [TestMethod]
        public async Task MetodoPagoRepository_Delete_WithValidMetodo_RemovesFromDatabase()
        {
            // Arrange
            var metodo = new MetodoPago { Nombre = "Bitcoin" };
            await _metodoPagoRepository.AddAsync(metodo);
            await _context.SaveChangesAsync();

            var addedMetodo = _context.Set<MetodoPago>().First();

            // Act
            _metodoPagoRepository.Delete(addedMetodo);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _metodoPagoRepository.GetByIdAsync(addedMetodo.Id);
            Assert.IsNull(result);
        }

        #endregion
    }
}
