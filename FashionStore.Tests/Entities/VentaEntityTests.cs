using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class VentaEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Venta_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var venta = new Venta();

            // Assert
            Assert.AreEqual(0, venta.Id);
            Assert.AreEqual(DateTime.Now.Date, venta.Fecha.Date);
            Assert.AreEqual(0, venta.ClienteId);
            Assert.IsNull(venta.Cliente);
            Assert.AreEqual(0, venta.VendedorId);
            Assert.IsNull(venta.Vendedor);
            Assert.AreEqual(0, venta.MetodoPagoId);
            Assert.IsNull(venta.MetodoPago);
            Assert.AreEqual(0m, venta.Total);
            Assert.IsNull(venta.DetalleVentas);
        }

        [TestMethod]
        public void Venta_SetProperties_StoresValues()
        {
            // Arrange
            var venta = new Venta();
            var fecha = DateTime.Now;

            // Act
            venta.Id = 1;
            venta.Fecha = fecha;
            venta.ClienteId = 1;
            venta.VendedorId = 1;
            venta.MetodoPagoId = 1;
            venta.Total = 99.99m;

            // Assert
            Assert.AreEqual(1, venta.Id);
            Assert.AreEqual(fecha, venta.Fecha);
            Assert.AreEqual(1, venta.ClienteId);
            Assert.AreEqual(1, venta.VendedorId);
            Assert.AreEqual(1, venta.MetodoPagoId);
            Assert.AreEqual(99.99m, venta.Total);
        }

        [TestMethod]
        public void Venta_Fecha_DefaultsToNow()
        {
            // Arrange & Act
            var venta = new Venta();
            var now = DateTime.Now;

            // Assert
            Assert.IsTrue((now - venta.Fecha).TotalSeconds < 1);
        }

        #endregion

        #region Navigation Tests

        [TestMethod]
        public void Venta_SetCliente_StoresReference()
        {
            // Arrange
            var venta = new Venta();
            var cliente = new Cliente { NombreCompleto = "Juan" };

            // Act
            venta.Cliente = cliente;

            // Assert
            Assert.IsNotNull(venta.Cliente);
            Assert.AreEqual("Juan", venta.Cliente.NombreCompleto);
        }

        [TestMethod]
        public void Venta_SetVendedor_StoresReference()
        {
            // Arrange
            var venta = new Venta();
            var vendedor = new Vendedor { Nombres = "Carlos" };

            // Act
            venta.Vendedor = vendedor;

            // Assert
            Assert.IsNotNull(venta.Vendedor);
            Assert.AreEqual("Carlos", venta.Vendedor.Nombres);
        }

        [TestMethod]
        public void Venta_SetMetodoPago_StoresReference()
        {
            // Arrange
            var venta = new Venta();
            var metodo = new MetodoPago { Nombre = "Tarjeta" };

            // Act
            venta.MetodoPago = metodo;

            // Assert
            Assert.IsNotNull(venta.MetodoPago);
            Assert.AreEqual("Tarjeta", venta.MetodoPago.Nombre);
        }

        [TestMethod]
        public void Venta_SetDetalleVentas_StoresCollection()
        {
            // Arrange
            var venta = new Venta();
            var detalles = new List<DetalleVenta>
            {
                new DetalleVenta { Id = 1 },
                new DetalleVenta { Id = 2 }
            };

            // Act
            venta.DetalleVentas = detalles;

            // Assert
            Assert.IsNotNull(venta.DetalleVentas);
            Assert.AreEqual(2, venta.DetalleVentas.Count);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Venta_TotalCanBeZero()
        {
            // Arrange & Act
            var venta = new Venta { Total = 0m };

            // Assert
            Assert.AreEqual(0m, venta.Total);
        }

        [TestMethod]
        public void Venta_TotalCanBeHighValue()
        {
            // Arrange & Act
            var venta = new Venta { Total = 99999.99m };

            // Assert
            Assert.AreEqual(99999.99m, venta.Total);
        }

        [TestMethod]
        public void Venta_DetalleVentas_CanBeNull()
        {
            // Arrange & Act
            var venta = new Venta { DetalleVentas = null };

            // Assert
            Assert.IsNull(venta.DetalleVentas);
        }

        #endregion
    }
}
