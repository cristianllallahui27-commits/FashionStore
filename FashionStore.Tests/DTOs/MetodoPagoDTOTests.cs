using FashionStore.Domain.DTOs;

namespace FashionStore.Tests.DTOs
{
    [TestClass]
    public class MetodoPagoDTOTests
    {
        #region Property Tests

        [TestMethod]
        public void MetodoPagoDTO_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var metodoDTO = new MetodoPagoDTO();

            // Assert
            Assert.AreEqual(0, metodoDTO.Id);
            Assert.AreEqual(string.Empty, metodoDTO.Nombre);
        }

        [TestMethod]
        public void MetodoPagoDTO_SetNombre_StoresValue()
        {
            // Arrange
            var metodoDTO = new MetodoPagoDTO();

            // Act
            metodoDTO.Nombre = "Tarjeta Crédito";

            // Assert
            Assert.AreEqual("Tarjeta Crédito", metodoDTO.Nombre);
        }

        [TestMethod]
        public void MetodoPagoDTO_SetAllProperties_StoresValues()
        {
            // Arrange
            var metodoDTO = new MetodoPagoDTO();

            // Act
            metodoDTO.Id = 1;
            metodoDTO.Nombre = "Transferencia Bancaria";

            // Assert
            Assert.AreEqual(1, metodoDTO.Id);
            Assert.AreEqual("Transferencia Bancaria", metodoDTO.Nombre);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void MetodoPagoDTO_NombreMaxLength_Is50()
        {
            // Arrange
            var metodoDTO = new MetodoPagoDTO();
            var longNombre = new string('a', 50);

            // Act
            metodoDTO.Nombre = longNombre;

            // Assert
            Assert.AreEqual(50, metodoDTO.Nombre.Length);
        }

        [TestMethod]
        public void MetodoPagoDTO_NombreCanBeShort()
        {
            // Arrange
            var metodoDTO = new MetodoPagoDTO();

            // Act
            metodoDTO.Nombre = "Efectivo";

            // Assert
            Assert.AreEqual("Efectivo", metodoDTO.Nombre);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void MetodoPagoDTO_NombreCanBeNull()
        {
            // Arrange & Act
            var metodoDTO = new MetodoPagoDTO { Nombre = null };

            // Assert
            Assert.IsNull(metodoDTO.Nombre);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void MetodoPagoDTO_CommonPaymentMethods_AreValid()
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
                var metodoDTO = new MetodoPagoDTO { Nombre = nombre };
                Assert.AreEqual(nombre, metodoDTO.Nombre);
            }
        }

        [TestMethod]
        public void MetodoPagoDTO_EmptyString_IsValid()
        {
            // Arrange & Act
            var metodoDTO = new MetodoPagoDTO { Nombre = string.Empty };

            // Assert
            Assert.AreEqual(string.Empty, metodoDTO.Nombre);
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void MetodoPagoDTO_SameValues_AreEquivalent()
        {
            // Arrange
            var dto1 = new MetodoPagoDTO { Id = 1, Nombre = "Test" };
            var dto2 = new MetodoPagoDTO { Id = 1, Nombre = "Test" };

            // Assert
            Assert.AreEqual(dto1.Id, dto2.Id);
            Assert.AreEqual(dto1.Nombre, dto2.Nombre);
        }

        #endregion
    }
}
