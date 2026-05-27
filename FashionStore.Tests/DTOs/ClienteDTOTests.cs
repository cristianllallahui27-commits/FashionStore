using FashionStore.Domain.DTOs;

namespace FashionStore.Tests.DTOs
{
    [TestClass]
    public class ClienteDTOTests
    {
        #region Property Tests

        [TestMethod]
        public void ClienteDTO_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO();

            // Assert
            Assert.AreEqual(0, clienteDTO.Id);
            Assert.IsNull(clienteDTO.NombreCompleto);
            Assert.IsNull(clienteDTO.DNI);
            Assert.IsNull(clienteDTO.Telefono);
            Assert.IsNull(clienteDTO.Direccion);
        }

        [TestMethod]
        public void ClienteDTO_SetAllProperties_StoresValues()
        {
            // Arrange
            var clienteDTO = new ClienteDTO();

            // Act
            clienteDTO.Id = 1;
            clienteDTO.NombreCompleto = "Juan Pérez";
            clienteDTO.DNI = "12345678";
            clienteDTO.Telefono = "555-1234";
            clienteDTO.Direccion = "Calle Principal 123";

            // Assert
            Assert.AreEqual(1, clienteDTO.Id);
            Assert.AreEqual("Juan Pérez", clienteDTO.NombreCompleto);
            Assert.AreEqual("12345678", clienteDTO.DNI);
            Assert.AreEqual("555-1234", clienteDTO.Telefono);
            Assert.AreEqual("Calle Principal 123", clienteDTO.Direccion);
        }

        [TestMethod]
        public void ClienteDTO_SetNombreCompleto_StoresValue()
        {
            // Arrange
            var clienteDTO = new ClienteDTO();

            // Act
            clienteDTO.NombreCompleto = "María García";

            // Assert
            Assert.AreEqual("María García", clienteDTO.NombreCompleto);
        }

        [TestMethod]
        public void ClienteDTO_SetDNI_StoresValue()
        {
            // Arrange
            var clienteDTO = new ClienteDTO();

            // Act
            clienteDTO.DNI = "87654321";

            // Assert
            Assert.AreEqual("87654321", clienteDTO.DNI);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void ClienteDTO_NombreCompletoCanBeNull()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO { NombreCompleto = null };

            // Assert
            Assert.IsNull(clienteDTO.NombreCompleto);
        }

        [TestMethod]
        public void ClienteDTO_DNICanBeNull()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO { DNI = null };

            // Assert
            Assert.IsNull(clienteDTO.DNI);
        }

        [TestMethod]
        public void ClienteDTO_TelefonoCanBeNull()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO { Telefono = null };

            // Assert
            Assert.IsNull(clienteDTO.Telefono);
        }

        [TestMethod]
        public void ClienteDTO_DireccionCanBeNull()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO { Direccion = null };

            // Assert
            Assert.IsNull(clienteDTO.Direccion);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void ClienteDTO_NombreCompletoWithMaxLength()
        {
            // Arrange
            var clienteDTO = new ClienteDTO();
            var longNombre = new string('a', 100);

            // Act
            clienteDTO.NombreCompleto = longNombre;

            // Assert
            Assert.AreEqual(100, clienteDTO.NombreCompleto.Length);
        }

        [TestMethod]
        public void ClienteDTO_DireccionWithMaxLength()
        {
            // Arrange
            var clienteDTO = new ClienteDTO();
            var longDireccion = new string('a', 250);

            // Act
            clienteDTO.Direccion = longDireccion;

            // Assert
            Assert.AreEqual(250, clienteDTO.Direccion.Length);
        }

        [TestMethod]
        public void ClienteDTO_EmptyStringsAreValid()
        {
            // Arrange & Act
            var clienteDTO = new ClienteDTO
            {
                NombreCompleto = string.Empty,
                DNI = string.Empty,
                Telefono = string.Empty,
                Direccion = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, clienteDTO.NombreCompleto);
            Assert.AreEqual(string.Empty, clienteDTO.DNI);
            Assert.AreEqual(string.Empty, clienteDTO.Telefono);
            Assert.AreEqual(string.Empty, clienteDTO.Direccion);
        }

        [TestMethod]
        public void ClienteDTO_ValidDNIFormats()
        {
            // Arrange
            var dnis = new[] { "12345678", "87654321", "00000000" };

            // Act & Assert
            foreach (var dni in dnis)
            {
                var clienteDTO = new ClienteDTO { DNI = dni };
                Assert.AreEqual(dni, clienteDTO.DNI);
            }
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void ClienteDTO_SameValues_AreEquivalent()
        {
            // Arrange
            var dto1 = new ClienteDTO { Id = 1, NombreCompleto = "Test" };
            var dto2 = new ClienteDTO { Id = 1, NombreCompleto = "Test" };

            // Assert
            Assert.AreEqual(dto1.Id, dto2.Id);
            Assert.AreEqual(dto1.NombreCompleto, dto2.NombreCompleto);
        }

        #endregion
    }
}
