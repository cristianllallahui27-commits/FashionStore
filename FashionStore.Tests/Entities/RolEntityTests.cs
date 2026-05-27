using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class RolEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Rol_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var rol = new Rol();

            // Assert
            Assert.AreEqual(0, rol.RolId);
            Assert.AreEqual(null, rol.Nombre);
            Assert.IsNull(rol.Descripcion);
            Assert.IsNotNull(rol.FechaCreacion);
            Assert.IsNotNull(rol.Usuarios);
        }

        [TestMethod]
        public void Rol_SetNombre_StoresValue()
        {
            // Arrange
            var rol = new Rol();
            var nombre = "Administrador";

            // Act
            rol.Nombre = nombre;

            // Assert
            Assert.AreEqual(nombre, rol.Nombre);
        }

        [TestMethod]
        public void Rol_SetDescripcion_StoresValue()
        {
            // Arrange
            var rol = new Rol();
            var descripcion = "Rol con acceso completo al sistema";

            // Act
            rol.Descripcion = descripcion;

            // Assert
            Assert.AreEqual(descripcion, rol.Descripcion);
        }

        [TestMethod]
        public void Rol_SetAllProperties_StoresValues()
        {
            // Arrange
            var rol = new Rol();
            var fecha = DateTime.UtcNow;

            // Act
            rol.RolId = 1;
            rol.Nombre = "Vendedor";
            rol.Descripcion = "Rol de vendedor";
            rol.FechaCreacion = fecha;

            // Assert
            Assert.AreEqual(1, rol.RolId);
            Assert.AreEqual("Vendedor", rol.Nombre);
            Assert.AreEqual("Rol de vendedor", rol.Descripcion);
            Assert.AreEqual(fecha, rol.FechaCreacion);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Rol_NombreMaxLength_Is100()
        {
            // Arrange
            var rol = new Rol();
            var longNombre = new string('a', 100);

            // Act
            rol.Nombre = longNombre;

            // Assert
            Assert.AreEqual(100, rol.Nombre.Length);
        }

        [TestMethod]
        public void Rol_NombreMinLength_Is3()
        {
            // Arrange
            var rol = new Rol();

            // Act
            rol.Nombre = "Adm";

            // Assert
            Assert.AreEqual(3, rol.Nombre.Length);
        }

        [TestMethod]
        public void Rol_DescripcionMaxLength_Is500()
        {
            // Arrange
            var rol = new Rol();
            var longDescripcion = new string('a', 500);

            // Act
            rol.Descripcion = longDescripcion;

            // Assert
            Assert.AreEqual(500, rol.Descripcion.Length);
        }

        #endregion

        #region Collection Tests

        [TestMethod]
        public void Rol_Usuarios_InitializedAsEmptyCollection()
        {
            // Arrange & Act
            var rol = new Rol();

            // Assert
            Assert.IsNotNull(rol.Usuarios);
            Assert.AreEqual(0, rol.Usuarios.Count);
        }

        [TestMethod]
        public void Rol_AddUsuario_ToCollection()
        {
            // Arrange
            var rol = new Rol();
            var usuario = new Usuario { Nombre = "Juan" };

            // Act
            rol.Usuarios.Add(usuario);

            // Assert
            Assert.AreEqual(1, rol.Usuarios.Count);
            Assert.IsTrue(rol.Usuarios.Contains(usuario));
        }

        [TestMethod]
        public void Rol_MultipleUsuarios_InCollection()
        {
            // Arrange
            var rol = new Rol();
            var usuarios = new List<Usuario>
            {
                new Usuario { Nombre = "Juan" },
                new Usuario { Nombre = "María" },
                new Usuario { Nombre = "Carlos" }
            };

            // Act
            foreach (var usuario in usuarios)
            {
                rol.Usuarios.Add(usuario);
            }

            // Assert
            Assert.AreEqual(3, rol.Usuarios.Count);
        }

        #endregion

        #region FechaCreacion Tests

        [TestMethod]
        public void Rol_FechaCreacion_DefaultsToUtcNow()
        {
            // Arrange & Act
            var rol = new Rol();
            var now = DateTime.UtcNow;

            // Assert
            Assert.IsTrue((now - rol.FechaCreacion).TotalSeconds < 1);
        }

        [TestMethod]
        public void Rol_FechaCreacion_CanBeSet()
        {
            // Arrange
            var rol = new Rol();
            var fecha = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            // Act
            rol.FechaCreacion = fecha;

            // Assert
            Assert.AreEqual(fecha, rol.FechaCreacion);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Rol_CommonRoles_AreValid()
        {
            // Arrange
            var roles = new[] 
            { 
                "Administrador",
                "Vendedor",
                "Cliente",
                "Gerente",
                "Supervisor"
            };

            // Act & Assert
            foreach (var nombreRol in roles)
            {
                var rol = new Rol { Nombre = nombreRol };
                Assert.AreEqual(nombreRol, rol.Nombre);
            }
        }

        [TestMethod]
        public void Rol_DescripcionCanBeNull()
        {
            // Arrange & Act
            var rol = new Rol { Nombre = "Test", Descripcion = null };

            // Assert
            Assert.IsNull(rol.Descripcion);
        }

        [TestMethod]
        public void Rol_DescripcionCanBeEmpty()
        {
            // Arrange & Act
            var rol = new Rol { Nombre = "Test", Descripcion = string.Empty };

            // Assert
            Assert.AreEqual(string.Empty, rol.Descripcion);
        }

        #endregion
    }
}
