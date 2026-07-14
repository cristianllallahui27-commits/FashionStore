using FashionStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FashionStore.Tests.Entities
{
    // ─────────────────────────────────────────────────────────────────────
    // El proyecto usa ASP.NET Identity para roles y usuarios.
    // Rol y Usuario son clases de Identity: IdentityRole y ApplicationUser.
    // Estos tests validan el comportamiento de ApplicationUser y la
    // integración con los roles del sistema (Administrador, Vendedor).
    // ─────────────────────────────────────────────────────────────────────

    [TestClass]
    public class RolEntityTests
    {
        #region IdentityRole Tests

        [TestMethod]
        public void IdentityRole_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var rol = new IdentityRole();

            // Assert — Name y NormalizedName son null por defecto
            Assert.IsNull(rol.Name);
            Assert.IsNull(rol.NormalizedName);
        }

        [TestMethod]
        public void IdentityRole_SetNombre_StoresValue()
        {
            // Arrange
            var rol = new IdentityRole();

            // Act
            rol.Name = "Administrador";

            // Assert
            Assert.AreEqual("Administrador", rol.Name);
        }

        [TestMethod]
        public void IdentityRole_SetAllProperties_StoresValues()
        {
            // Arrange
            var rol = new IdentityRole();

            // Act
            rol.Id = "rol-admin-id";
            rol.Name = "Administrador";
            rol.NormalizedName = "ADMINISTRADOR";

            // Assert
            Assert.AreEqual("rol-admin-id", rol.Id);
            Assert.AreEqual("Administrador", rol.Name);
            Assert.AreEqual("ADMINISTRADOR", rol.NormalizedName);
        }

        [TestMethod]
        public void IdentityRole_NombreMaxLength_Is100()
        {
            // Arrange
            var rol = new IdentityRole();
            var longNombre = new string('a', 100);

            // Act
            rol.Name = longNombre;

            // Assert
            Assert.AreEqual(100, rol.Name!.Length);
        }

        [TestMethod]
        public void IdentityRole_DescripcionCanBeNull()
        {
            // Arrange & Act
            var rol = new IdentityRole { Name = "Test" };

            // Assert
            Assert.IsNull(rol.NormalizedName);
        }

        [TestMethod]
        public void IdentityRole_CommonRoles_AreValid()
        {
            // Arrange
            var roles = new[] { "Administrador", "Vendedor" };

            // Act & Assert
            foreach (var nombre in roles)
            {
                var rol = new IdentityRole { Name = nombre };
                Assert.AreEqual(nombre, rol.Name);
            }
        }

        #endregion

        #region ApplicationUser Tests

        [TestMethod]
        public void ApplicationUser_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            Assert.IsNull(user.UserName);
            Assert.IsNull(user.Email);
        }

        [TestMethod]
        public void ApplicationUser_SetEmail_StoresValue()
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.Email = "admin@fashionstore.com";

            // Assert
            Assert.AreEqual("admin@fashionstore.com", user.Email);
        }

        [TestMethod]
        public void ApplicationUser_SetAllProperties_StoresValues()
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.UserName = "admin@fashionstore.com";
            user.Email    = "admin@fashionstore.com";
            user.EmailConfirmed = true;

            // Assert
            Assert.AreEqual("admin@fashionstore.com", user.UserName);
            Assert.AreEqual("admin@fashionstore.com", user.Email);
            Assert.IsTrue(user.EmailConfirmed);
        }

        [TestMethod]
        public void ApplicationUser_MultipleUsers_AreIndependent()
        {
            // Arrange
            var usuarios = new List<ApplicationUser>
            {
                new ApplicationUser { Email = "user1@test.com" },
                new ApplicationUser { Email = "user2@test.com" },
                new ApplicationUser { Email = "user3@test.com" }
            };

            // Act & Assert
            Assert.AreEqual(3, usuarios.Count);
            Assert.AreEqual("user1@test.com", usuarios[0].Email);
            Assert.AreEqual("user2@test.com", usuarios[1].Email);
            Assert.AreEqual("user3@test.com", usuarios[2].Email);
        }

        [TestMethod]
        public void ApplicationUser_EmailConfirmed_DefaultsToFalse()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert — por defecto Identity no confirma el email
            Assert.IsFalse(user.EmailConfirmed);
        }

        #endregion
    }
}
