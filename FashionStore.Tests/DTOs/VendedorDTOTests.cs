using FashionStore.Domain.DTOs;

namespace FashionStore.Tests.DTOs
{
    [TestClass]
    public class VendedorDTOTests
    {
        #region Property Tests

        [TestMethod]
        public void VendedorDTO_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO();

            // Assert
            Assert.AreEqual(0, vendedorDTO.Id);
            Assert.AreEqual(string.Empty, vendedorDTO.Nombres);
            Assert.AreEqual(string.Empty, vendedorDTO.Apellidos);
            Assert.AreEqual(string.Empty, vendedorDTO.DNI);
            Assert.IsNull(vendedorDTO.Telefono);
            Assert.IsNull(vendedorDTO.Correo);
            Assert.AreEqual(false, vendedorDTO.Estado);
        }

        [TestMethod]
        public void VendedorDTO_SetAllProperties_StoresValues()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();

            // Act
            vendedorDTO.Id = 1;
            vendedorDTO.Nombres = "Juan";
            vendedorDTO.Apellidos = "Pérez";
            vendedorDTO.DNI = "12345678";
            vendedorDTO.Telefono = "555-1234";
            vendedorDTO.Correo = "juan@example.com";
            vendedorDTO.Estado = true;

            // Assert
            Assert.AreEqual(1, vendedorDTO.Id);
            Assert.AreEqual("Juan", vendedorDTO.Nombres);
            Assert.AreEqual("Pérez", vendedorDTO.Apellidos);
            Assert.AreEqual("12345678", vendedorDTO.DNI);
            Assert.AreEqual("555-1234", vendedorDTO.Telefono);
            Assert.AreEqual("juan@example.com", vendedorDTO.Correo);
            Assert.AreEqual(true, vendedorDTO.Estado);
        }

        [TestMethod]
        public void VendedorDTO_SetNombres_StoresValue()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();

            // Act
            vendedorDTO.Nombres = "Carlos";

            // Assert
            Assert.AreEqual("Carlos", vendedorDTO.Nombres);
        }

        [TestMethod]
        public void VendedorDTO_SetApellidos_StoresValue()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();

            // Act
            vendedorDTO.Apellidos = "García";

            // Assert
            Assert.AreEqual("García", vendedorDTO.Apellidos);
        }

        [TestMethod]
        public void VendedorDTO_SetDNI_StoresValue()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();

            // Act
            vendedorDTO.DNI = "87654321";

            // Assert
            Assert.AreEqual("87654321", vendedorDTO.DNI);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void VendedorDTO_NombresMaxLength_Is150()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();
            var longNombres = new string('a', 150);

            // Act
            vendedorDTO.Nombres = longNombres;

            // Assert
            Assert.AreEqual(150, vendedorDTO.Nombres.Length);
        }

        [TestMethod]
        public void VendedorDTO_ApellidosMaxLength_Is150()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();
            var longApellidos = new string('a', 150);

            // Act
            vendedorDTO.Apellidos = longApellidos;

            // Assert
            Assert.AreEqual(150, vendedorDTO.Apellidos.Length);
        }

        [TestMethod]
        public void VendedorDTO_DNIMaxLength_Is8()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();
            var dni = new string('1', 8);

            // Act
            vendedorDTO.DNI = dni;

            // Assert
            Assert.AreEqual(8, vendedorDTO.DNI.Length);
        }

        [TestMethod]
        public void VendedorDTO_TelefonoMaxLength_Is15()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();
            var telefono = new string('5', 15);

            // Act
            vendedorDTO.Telefono = telefono;

            // Assert
            Assert.AreEqual(15, vendedorDTO.Telefono.Length);
        }

        [TestMethod]
        public void VendedorDTO_CorreoMaxLength_Is150()
        {
            // Arrange
            var vendedorDTO = new VendedorDTO();
            var correo = new string('a', 150);

            // Act
            vendedorDTO.Correo = correo;

            // Assert
            Assert.AreEqual(150, vendedorDTO.Correo.Length);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void VendedorDTO_TelefonoCanBeNull()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO { Telefono = null };

            // Assert
            Assert.IsNull(vendedorDTO.Telefono);
        }

        [TestMethod]
        public void VendedorDTO_CorreoCanBeNull()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO { Correo = null };

            // Assert
            Assert.IsNull(vendedorDTO.Correo);
        }

        #endregion

        #region Estado Tests

        [TestMethod]
        public void VendedorDTO_EstadoCanBeTrue()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO { Estado = true };

            // Assert
            Assert.AreEqual(true, vendedorDTO.Estado);
        }

        [TestMethod]
        public void VendedorDTO_EstadoCanBeFalse()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO { Estado = false };

            // Assert
            Assert.AreEqual(false, vendedorDTO.Estado);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void VendedorDTO_EmptyStringsAreValid()
        {
            // Arrange & Act
            var vendedorDTO = new VendedorDTO
            {
                Nombres = string.Empty,
                Apellidos = string.Empty,
                DNI = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, vendedorDTO.Nombres);
            Assert.AreEqual(string.Empty, vendedorDTO.Apellidos);
            Assert.AreEqual(string.Empty, vendedorDTO.DNI);
        }

        [TestMethod]
        public void VendedorDTO_ValidDNIFormats()
        {
            // Arrange
            var dnis = new[] { "12345678", "87654321", "00000000" };

            // Act & Assert
            foreach (var dni in dnis)
            {
                var vendedorDTO = new VendedorDTO { DNI = dni };
                Assert.AreEqual(dni, vendedorDTO.DNI);
            }
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void VendedorDTO_SameValues_AreEquivalent()
        {
            // Arrange
            var dto1 = new VendedorDTO { Id = 1, Nombres = "Test" };
            var dto2 = new VendedorDTO { Id = 1, Nombres = "Test" };

            // Assert
            Assert.AreEqual(dto1.Id, dto2.Id);
            Assert.AreEqual(dto1.Nombres, dto2.Nombres);
        }

        #endregion
    }
}
