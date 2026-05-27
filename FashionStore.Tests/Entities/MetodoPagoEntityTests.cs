using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class MetodoPagoEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void MetodoPago_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var metodo = new MetodoPago();

            // Assert
            Assert.AreEqual(0, metodo.Id);
            Assert.AreEqual(string.Empty, metodo.Nombre);
            Assert.IsNull(metodo.Ventas);
        }

        [TestMethod]
        public void MetodoPago_SetNombre_StoresValue()
        {
            // Arrange
            var metodo = new MetodoPago();
            var nombre = "Tarjeta Crédito";

            // Act
            metodo.Nombre = nombre;

            // Assert
            Assert.AreEqual(nombre, metodo.Nombre);
        }

        [TestMethod]
        public void MetodoPago_SetAllProperties_StoresValues()
        {
            // Arrange
            var metodo = new MetodoPago();

            // Act
            metodo.Id = 1;
            metodo.Nombre = "Transferencia Bancaria";

            // Assert
            Assert.AreEqual(1, metodo.Id);
            Assert.AreEqual("Transferencia Bancaria", metodo.Nombre);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void MetodoPago_NombreMaxLength_Is50()
        {
            // Arrange
            var metodo = new MetodoPago();
            var longNombre = new string('a', 50);

            // Act
            metodo.Nombre = longNombre;

            // Assert
            Assert.AreEqual(50, metodo.Nombre.Length);
        }

        [TestMethod]
        public void MetodoPago_NombreCanBeShort()
        {
            // Arrange
            var metodo = new MetodoPago();

            // Act
            metodo.Nombre = "Efectivo";

            // Assert
            Assert.AreEqual("Efectivo", metodo.Nombre);
        }

        #endregion

        #region Collection Tests

        [TestMethod]
        public void MetodoPago_SetVentas_StoresCollection()
        {
            // Arrange
            var metodo = new MetodoPago();
            var ventas = new List<Venta>
            {
                new Venta { Id = 1 },
                new Venta { Id = 2 }
            };

            // Act
            metodo.Ventas = ventas;

            // Assert
            Assert.IsNotNull(metodo.Ventas);
            Assert.AreEqual(2, metodo.Ventas.Count);
        }

        [TestMethod]
        public void MetodoPago_Ventas_CanBeNull()
        {
            // Arrange & Act
            var metodo = new MetodoPago { Ventas = null };

            // Assert
            Assert.IsNull(metodo.Ventas);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void MetodoPago_CommonPaymentMethods_AreValid()
        {
            // Arrange
            var metodos = new[] 
            { 
                "Efectivo",
                "Tarjeta Crédito",
                "Tarjeta Débito",
                "Transferencia",
                "Cheque"
            };

            // Act & Assert
            foreach (var nombre in metodos)
            {
                var metodo = new MetodoPago { Nombre = nombre };
                Assert.AreEqual(nombre, metodo.Nombre);
            }
        }

        [TestMethod]
        public void MetodoPago_EmptyNombre_IsValid()
        {
            // Arrange & Act
            var metodo = new MetodoPago { Nombre = string.Empty };

            // Assert
            Assert.AreEqual(string.Empty, metodo.Nombre);
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void MetodoPago_SameName_CanBeDifferent()
        {
            // Arrange
            var metodo1 = new MetodoPago { Id = 1, Nombre = "Efectivo" };
            var metodo2 = new MetodoPago { Id = 2, Nombre = "Efectivo" };

            // Assert
            Assert.AreNotEqual(metodo1.Id, metodo2.Id);
            Assert.AreEqual(metodo1.Nombre, metodo2.Nombre);
        }

        #endregion
    }
}
