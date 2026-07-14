using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class ClienteEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Cliente_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var cliente = new Cliente();

            // Assert
            Assert.AreEqual(0, cliente.Id);
            Assert.AreEqual(string.Empty, cliente.NombreCompleto);
            Assert.AreEqual(string.Empty, cliente.DNI);
            Assert.AreEqual(string.Empty, cliente.Telefono);
            Assert.AreEqual(string.Empty, cliente.Direccion);
            Assert.IsNull(cliente.Email);
        }

        [TestMethod]
        public void Cliente_SetAllProperties_StoresValues()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Id = 1;
            cliente.NombreCompleto = "Juan Pérez García";
            cliente.DNI = "12345678";
            cliente.Telefono = "555-1234";
            cliente.Direccion = "Calle Principal 123, Apt 4B";

            // Assert
            Assert.AreEqual(1, cliente.Id);
            Assert.AreEqual("Juan Pérez García", cliente.NombreCompleto);
            Assert.AreEqual("12345678", cliente.DNI);
            Assert.AreEqual("555-1234", cliente.Telefono);
            Assert.AreEqual("Calle Principal 123, Apt 4B", cliente.Direccion);
        }

        [TestMethod]
        public void Cliente_SetNombreCompleto_StoresValue()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.NombreCompleto = "María García";

            // Assert
            Assert.AreEqual("María García", cliente.NombreCompleto);
        }

        [TestMethod]
        public void Cliente_SetDNI_StoresValue()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.DNI = "87654321";

            // Assert
            Assert.AreEqual("87654321", cliente.DNI);
        }

        [TestMethod]
        public void Cliente_SetTelefono_StoresValue()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Telefono = "555-9999";

            // Assert
            Assert.AreEqual("555-9999", cliente.Telefono);
        }

        [TestMethod]
        public void Cliente_SetDireccion_StoresValue()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Direccion = "Avenida Secundaria 456";

            // Assert
            Assert.AreEqual("Avenida Secundaria 456", cliente.Direccion);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Cliente_NombreCompletoCanBeVeryLong()
        {
            // Arrange
            var cliente = new Cliente();
            var longNombre = "Juan Carlos María Antonio Pérez García López Rodríguez Fernández";

            // Act
            cliente.NombreCompleto = longNombre;

            // Assert
            Assert.AreEqual(longNombre, cliente.NombreCompleto);
        }

        [TestMethod]
        public void Cliente_DNIValidFormats()
        {
            // Arrange
            var clientes = new[]
            {
                "12345678",
                "87654321",
                "00000000",
                "99999999"
            };

            // Act & Assert
            foreach (var dni in clientes)
            {
                var cliente = new Cliente { DNI = dni };
                Assert.AreEqual(dni, cliente.DNI);
            }
        }

        [TestMethod]
        public void Cliente_TelefonoCanBeEmpty()
        {
            // Arrange & Act
            var cliente = new Cliente { Telefono = string.Empty };

            // Assert
            Assert.AreEqual(string.Empty, cliente.Telefono);
        }

        [TestMethod]
        public void Cliente_DireccionCanBeVeryLong()
        {
            // Arrange
            var cliente = new Cliente();
            var longDireccion = "Calle Larga del Barrio Antiguo, Número 1234, Apto 5678, Ciudad Importante";

            // Act
            cliente.Direccion = longDireccion;

            // Assert
            Assert.AreEqual(longDireccion, cliente.Direccion);
        }

        [TestMethod]
        public void Cliente_AllPropertiesCanBeEmpty()
        {
            // Arrange & Act
            var cliente = new Cliente
            {
                NombreCompleto = string.Empty,
                DNI = string.Empty,
                Telefono = string.Empty,
                Direccion = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, cliente.NombreCompleto);
            Assert.AreEqual(string.Empty, cliente.DNI);
            Assert.AreEqual(string.Empty, cliente.Telefono);
            Assert.AreEqual(string.Empty, cliente.Direccion);
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void Cliente_SameName_CanBeDifferent()
        {
            // Arrange
            var cliente1 = new Cliente { Id = 1, NombreCompleto = "Juan" };
            var cliente2 = new Cliente { Id = 2, NombreCompleto = "Juan" };

            // Assert
            Assert.AreNotEqual(cliente1.Id, cliente2.Id);
            Assert.AreEqual(cliente1.NombreCompleto, cliente2.NombreCompleto);
        }

        #endregion
    }
}
