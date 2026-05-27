using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class DetalleVentaEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void DetalleVenta_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var detalle = new DetalleVenta();

            // Assert
            Assert.AreEqual(0, detalle.Id);
            Assert.AreEqual(0, detalle.VentaId);
            Assert.IsNull(detalle.Venta);
            Assert.AreEqual(0, detalle.PrendaId);
            Assert.IsNull(detalle.Prenda);
            Assert.AreEqual(0, detalle.Cantidad);
            Assert.AreEqual(0m, detalle.Precio);
            Assert.AreEqual(0m, detalle.Subtotal);
        }

        [TestMethod]
        public void DetalleVenta_SetAllProperties_StoresValues()
        {
            // Arrange
            var detalle = new DetalleVenta();

            // Act
            detalle.Id = 1;
            detalle.VentaId = 1;
            detalle.PrendaId = 1;
            detalle.Cantidad = 5;
            detalle.Precio = 29.99m;
            detalle.Subtotal = 149.95m;

            // Assert
            Assert.AreEqual(1, detalle.Id);
            Assert.AreEqual(1, detalle.VentaId);
            Assert.AreEqual(1, detalle.PrendaId);
            Assert.AreEqual(5, detalle.Cantidad);
            Assert.AreEqual(29.99m, detalle.Precio);
            Assert.AreEqual(149.95m, detalle.Subtotal);
        }

        #endregion

        #region Navigation Tests

        [TestMethod]
        public void DetalleVenta_SetVenta_StoresReference()
        {
            // Arrange
            var detalle = new DetalleVenta();
            var venta = new Venta { Id = 1, Total = 100m };

            // Act
            detalle.Venta = venta;

            // Assert
            Assert.IsNotNull(detalle.Venta);
            Assert.AreEqual(100m, detalle.Venta.Total);
        }

        [TestMethod]
        public void DetalleVenta_SetPrenda_StoresReference()
        {
            // Arrange
            var detalle = new DetalleVenta();
            var prenda = new Prenda { Nombre = "Camiseta" };

            // Act
            detalle.Prenda = prenda;

            // Assert
            Assert.IsNotNull(detalle.Prenda);
            Assert.AreEqual("Camiseta", detalle.Prenda.Nombre);
        }

        #endregion

        #region Price and Quantity Tests

        [TestMethod]
        public void DetalleVenta_CantidadCanBeZero()
        {
            // Arrange & Act
            var detalle = new DetalleVenta { Cantidad = 0 };

            // Assert
            Assert.AreEqual(0, detalle.Cantidad);
        }

        [TestMethod]
        public void DetalleVenta_CantidadCanBeHigh()
        {
            // Arrange & Act
            var detalle = new DetalleVenta { Cantidad = 1000 };

            // Assert
            Assert.AreEqual(1000, detalle.Cantidad);
        }

        [TestMethod]
        public void DetalleVenta_PrecioCanBeDecimal()
        {
            // Arrange & Act
            var detalle = new DetalleVenta { Precio = 29.99m };

            // Assert
            Assert.AreEqual(29.99m, detalle.Precio);
        }

        [TestMethod]
        public void DetalleVenta_SubtotalCalculation_IsCorrect()
        {
            // Arrange
            var detalle = new DetalleVenta 
            { 
                Cantidad = 5, 
                Precio = 10.00m,
                Subtotal = 50.00m
            };

            // Act & Assert
            Assert.AreEqual(50.00m, detalle.Subtotal);
        }

        #endregion

        #region Venta-Prenda Relationship Tests

        [TestMethod]
        public void DetalleVenta_VentaIdMatchesVentaNavigation()
        {
            // Arrange
            var detalle = new DetalleVenta { VentaId = 5 };
            var venta = new Venta { Id = 5 };

            // Act
            detalle.Venta = venta;

            // Assert
            Assert.AreEqual(detalle.VentaId, detalle.Venta.Id);
        }

        [TestMethod]
        public void DetalleVenta_PrendaIdMatchesPrendaNavigation()
        {
            // Arrange
            var detalle = new DetalleVenta { PrendaId = 3 };
            var prenda = new Prenda { Id = 3 };

            // Act
            detalle.Prenda = prenda;

            // Assert
            Assert.AreEqual(detalle.PrendaId, detalle.Prenda.Id);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void DetalleVenta_NullReferences_AreAllowed()
        {
            // Arrange & Act
            var detalle = new DetalleVenta 
            { 
                Venta = null, 
                Prenda = null 
            };

            // Assert
            Assert.IsNull(detalle.Venta);
            Assert.IsNull(detalle.Prenda);
        }

        [TestMethod]
        public void DetalleVenta_MaxValues_AllowedForAllProperties()
        {
            // Arrange & Act
            var detalle = new DetalleVenta
            {
                Cantidad = int.MaxValue,
                Precio = 99999.99m,
                Subtotal = 99999.99m
            };

            // Assert
            Assert.AreEqual(int.MaxValue, detalle.Cantidad);
            Assert.AreEqual(99999.99m, detalle.Precio);
            Assert.AreEqual(99999.99m, detalle.Subtotal);
        }

        #endregion
    }
}
