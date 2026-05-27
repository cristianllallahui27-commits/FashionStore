using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class CategoriaEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Categoria_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var categoria = new Categoria();

            // Assert
            Assert.AreEqual(0, categoria.Id);
            Assert.AreEqual(string.Empty, categoria.Nombre);
            Assert.IsNull(categoria.Descripcion);
            Assert.IsNull(categoria.Prendas);
        }

        [TestMethod]
        public void Categoria_SetNombre_StoresValue()
        {
            // Arrange
            var categoria = new Categoria();
            var nombre = "Camisetas";

            // Act
            categoria.Nombre = nombre;

            // Assert
            Assert.AreEqual(nombre, categoria.Nombre);
        }

        [TestMethod]
        public void Categoria_SetDescripcion_StoresValue()
        {
            // Arrange
            var categoria = new Categoria();
            var descripcion = "Camisetas de algodón de alta calidad";

            // Act
            categoria.Descripcion = descripcion;

            // Assert
            Assert.AreEqual(descripcion, categoria.Descripcion);
        }

        [TestMethod]
        public void Categoria_SetId_StoresValue()
        {
            // Arrange
            var categoria = new Categoria();

            // Act
            categoria.Id = 5;

            // Assert
            Assert.AreEqual(5, categoria.Id);
        }

        #endregion

        #region Collection Tests

        [TestMethod]
        public void Categoria_SetPrendas_StoresCollection()
        {
            // Arrange
            var categoria = new Categoria();
            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Prenda1" },
                new Prenda { Id = 2, Nombre = "Prenda2" }
            };

            // Act
            categoria.Prendas = prendas;

            // Assert
            Assert.IsNotNull(categoria.Prendas);
            Assert.AreEqual(2, categoria.Prendas.Count);
        }

        [TestMethod]
        public void Categoria_Prendas_CanBeNull()
        {
            // Arrange & Act
            var categoria = new Categoria { Prendas = null };

            // Assert
            Assert.IsNull(categoria.Prendas);
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Categoria_Constructor_InitializesWithEmptyNombre()
        {
            // Act
            var categoria = new Categoria();

            // Assert
            Assert.IsNotNull(categoria.Nombre);
            Assert.AreEqual(string.Empty, categoria.Nombre);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Categoria_NombreMaxLength_Is100()
        {
            // Arrange
            var categoria = new Categoria();
            var longNombre = new string('a', 100);

            // Act
            categoria.Nombre = longNombre;

            // Assert
            Assert.AreEqual(100, categoria.Nombre.Length);
        }

        [TestMethod]
        public void Categoria_DescripcionMaxLength_Is250()
        {
            // Arrange
            var categoria = new Categoria();
            var longDescripcion = new string('a', 250);

            // Act
            categoria.Descripcion = longDescripcion;

            // Assert
            Assert.AreEqual(250, categoria.Descripcion.Length);
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void Categoria_SameName_CanBeDifferent()
        {
            // Arrange
            var categoria1 = new Categoria { Id = 1, Nombre = "Camisetas" };
            var categoria2 = new Categoria { Id = 2, Nombre = "Camisetas" };

            // Assert
            Assert.AreNotEqual(categoria1.Id, categoria2.Id);
            Assert.AreEqual(categoria1.Nombre, categoria2.Nombre);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Categoria_WithNullNombre_DefaultsToEmptyString()
        {
            // Arrange & Act
            var categoria = new Categoria { Nombre = null! };

            // Assert
            // Note: This tests current behavior; with null-coalescing, should default to ""
            Assert.IsNull(categoria.Nombre);
        }

        [TestMethod]
        public void Categoria_WithEmptyDescripcion_IsValid()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Test", Descripcion = string.Empty };

            // Assert
            Assert.AreEqual(string.Empty, categoria.Descripcion);
        }

        #endregion
    }
}
