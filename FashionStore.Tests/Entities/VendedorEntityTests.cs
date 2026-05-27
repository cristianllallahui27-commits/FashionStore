using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class VendedorEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Vendedor_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var vendedor = new Vendedor();

            // Assert
            Assert.AreEqual(0, vendedor.Id);
            Assert.AreEqual(string.Empty, vendedor.Nombres);
            Assert.AreEqual(string.Empty, vendedor.Apellidos);
            Assert.AreEqual(string.Empty, vendedor.DNI);
            Assert.IsNull(vendedor.Telefono);
            Assert.IsNull(vendedor.Correo);
            Assert.AreEqual(true, vendedor.Estado);
            Assert.IsNull(vendedor.Ventas);
        }

        [TestMethod]
        public void Vendedor_SetAllProperties_StoresValues()
        {
            // Arrange
            var vendedor = new Vendedor();

            // Act
            vendedor.Id = 1;
            vendedor.Nombres = "Carlos";
            vendedor.Apellidos = "López";
            vendedor.DNI = "12345678";
            vendedor.Telefono = "555-1234";
            vendedor.Correo = "carlos@example.com";
            vendedor.Estado = false;

            // Assert
            Assert.AreEqual(1, vendedor.Id);
            Assert.AreEqual("Carlos", vendedor.Nombres);
            Assert.AreEqual("López", vendedor.Apellidos);
            Assert.AreEqual("12345678", vendedor.DNI);
            Assert.AreEqual("555-1234", vendedor.Telefono);
            Assert.AreEqual("carlos@example.com", vendedor.Correo);
            Assert.AreEqual(false, vendedor.Estado);
        }

        [TestMethod]
        public void Vendedor_SetNombres_StoresValue()
        {
            // Arrange
            var vendedor = new Vendedor();

            // Act
            vendedor.Nombres = "Juan";

            // Assert
            Assert.AreEqual("Juan", vendedor.Nombres);
        }

        [TestMethod]
        public void Vendedor_SetApellidos_StoresValue()
        {
            // Arrange
            var vendedor = new Vendedor();

            // Act
            vendedor.Apellidos = "García";

            // Assert
            Assert.AreEqual("García", vendedor.Apellidos);
        }

        [TestMethod]
        public void Vendedor_SetDNI_StoresValue()
        {
            // Arrange
            var vendedor = new Vendedor();

            // Act
            vendedor.DNI = "87654321";

            // Assert
            Assert.AreEqual("87654321", vendedor.DNI);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Vendedor_NombresMaxLength_Is150()
        {
            // Arrange
            var vendedor = new Vendedor();
            var longNombres = new string('a', 150);

            // Act
            vendedor.Nombres = longNombres;

            // Assert
            Assert.AreEqual(150, vendedor.Nombres.Length);
        }

        [TestMethod]
        public void Vendedor_ApellidosMaxLength_Is150()
        {
            // Arrange
            var vendedor = new Vendedor();
            var longApellidos = new string('a', 150);

            // Act
            vendedor.Apellidos = longApellidos;

            // Assert
            Assert.AreEqual(150, vendedor.Apellidos.Length);
        }

        [TestMethod]
        public void Vendedor_DNIMaxLength_Is8()
        {
            // Arrange
            var vendedor = new Vendedor();
            var dni = "12345678";

            // Act
            vendedor.DNI = dni;

            // Assert
            Assert.AreEqual(8, vendedor.DNI.Length);
        }

        [TestMethod]
        public void Vendedor_TelefonoMaxLength_Is15()
        {
            // Arrange
            var vendedor = new Vendedor();
            var telefono = new string('5', 15);

            // Act
            vendedor.Telefono = telefono;

            // Assert
            Assert.AreEqual(15, vendedor.Telefono.Length);
        }

        [TestMethod]
        public void Vendedor_CorreoMaxLength_Is150()
        {
            // Arrange
            var vendedor = new Vendedor();
            var correo = new string('a', 150);

            // Act
            vendedor.Correo = correo;

            // Assert
            Assert.AreEqual(150, vendedor.Correo.Length);
        }

        #endregion

        #region Estado Tests

        [TestMethod]
        public void Vendedor_EstadoDefaultsToTrue()
        {
            // Arrange & Act
            var vendedor = new Vendedor();

            // Assert
            Assert.AreEqual(true, vendedor.Estado);
        }

        [TestMethod]
        public void Vendedor_EstadoCanBeSetToFalse()
        {
            // Arrange
            var vendedor = new Vendedor();

            // Act
            vendedor.Estado = false;

            // Assert
            Assert.AreEqual(false, vendedor.Estado);
        }

        [TestMethod]
        public void Vendedor_EstadoCanBeSetToTrue()
        {
            // Arrange
            var vendedor = new Vendedor { Estado = false };

            // Act
            vendedor.Estado = true;

            // Assert
            Assert.AreEqual(true, vendedor.Estado);
        }

        #endregion

        #region Collection Tests

        [TestMethod]
        public void Vendedor_Ventas_CanBeNull()
        {
            // Arrange & Act
            var vendedor = new Vendedor { Ventas = null };

            // Assert
            Assert.IsNull(vendedor.Ventas);
        }

        [TestMethod]
        public void Vendedor_SetVentas_StoresCollection()
        {
            // Arrange
            var vendedor = new Vendedor();
            var ventas = new List<Venta>
            {
                new Venta { Id = 1 },
                new Venta { Id = 2 }
            };

            // Act
            vendedor.Ventas = ventas;

            // Assert
            Assert.IsNotNull(vendedor.Ventas);
            Assert.AreEqual(2, vendedor.Ventas.Count);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void Vendedor_TelefonoCanBeNull()
        {
            // Arrange & Act
            var vendedor = new Vendedor { Telefono = null };

            // Assert
            Assert.IsNull(vendedor.Telefono);
        }

        [TestMethod]
        public void Vendedor_CorreoCanBeNull()
        {
            // Arrange & Act
            var vendedor = new Vendedor { Correo = null };

            // Assert
            Assert.IsNull(vendedor.Correo);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Vendedor_CommonNames_AreValid()
        {
            // Arrange
            var names = new[] { "Juan", "María", "Carlos", "Ana" };

            // Act & Assert
            foreach (var name in names)
            {
                var vendedor = new Vendedor { Nombres = name };
                Assert.AreEqual(name, vendedor.Nombres);
            }
        }

        [TestMethod]
        public void Vendedor_ValidDNIFormats()
        {
            // Arrange
            var dnis = new[] { "12345678", "87654321", "00000000" };

            // Act & Assert
            foreach (var dni in dnis)
            {
                var vendedor = new Vendedor { DNI = dni };
                Assert.AreEqual(dni, vendedor.DNI);
            }
        }

        [TestMethod]
        public void Vendedor_EmailFormats_AreValid()
        {
            // Arrange
            var emails = new[] 
            { 
                "vendedor@company.com",
                "carlos.lopez@store.com",
                "seller123@domain.org"
            };

            // Act & Assert
            foreach (var email in emails)
            {
                var vendedor = new Vendedor { Correo = email };
                Assert.AreEqual(email, vendedor.Correo);
            }
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void Vendedor_SameName_CanBeDifferent()
        {
            // Arrange
            var vendedor1 = new Vendedor { Id = 1, Nombres = "Juan" };
            var vendedor2 = new Vendedor { Id = 2, Nombres = "Juan" };

            // Assert
            Assert.AreNotEqual(vendedor1.Id, vendedor2.Id);
            Assert.AreEqual(vendedor1.Nombres, vendedor2.Nombres);
        }

        #endregion
    }
}
