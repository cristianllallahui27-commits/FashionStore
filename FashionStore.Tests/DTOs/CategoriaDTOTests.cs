using FashionStore.Domain.DTOs;

namespace FashionStore.Tests.DTOs
{
    [TestClass]
    public class CategoriaDTOTests
    {
        #region Property Tests

        [TestMethod]
        public void CategoriaDTO_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var categoriaDTO = new CategoriaDTO();

            // Assert
            Assert.AreEqual(0, categoriaDTO.Id);
            Assert.IsNull(categoriaDTO.Nombre);
            Assert.IsNull(categoriaDTO.Descripcion);
        }

        [TestMethod]
        public void CategoriaDTO_SetId_StoresValue()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();

            // Act
            categoriaDTO.Id = 5;

            // Assert
            Assert.AreEqual(5, categoriaDTO.Id);
        }

        [TestMethod]
        public void CategoriaDTO_SetNombre_StoresValue()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();

            // Act
            categoriaDTO.Nombre = "Camisetas";

            // Assert
            Assert.AreEqual("Camisetas", categoriaDTO.Nombre);
        }

        [TestMethod]
        public void CategoriaDTO_SetDescripcion_StoresValue()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();

            // Act
            categoriaDTO.Descripcion = "Camisetas variadas";

            // Assert
            Assert.AreEqual("Camisetas variadas", categoriaDTO.Descripcion);
        }

        [TestMethod]
        public void CategoriaDTO_SetAllProperties_StoresAll()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();

            // Act
            categoriaDTO.Id = 1;
            categoriaDTO.Nombre = "Ropa";
            categoriaDTO.Descripcion = "Ropa de calidad";

            // Assert
            Assert.AreEqual(1, categoriaDTO.Id);
            Assert.AreEqual("Ropa", categoriaDTO.Nombre);
            Assert.AreEqual("Ropa de calidad", categoriaDTO.Descripcion);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void CategoriaDTO_NombreCanBeNull()
        {
            // Arrange & Act
            var categoriaDTO = new CategoriaDTO { Nombre = null };

            // Assert
            Assert.IsNull(categoriaDTO.Nombre);
        }

        [TestMethod]
        public void CategoriaDTO_DescripcionCanBeNull()
        {
            // Arrange & Act
            var categoriaDTO = new CategoriaDTO { Descripcion = null };

            // Assert
            Assert.IsNull(categoriaDTO.Descripcion);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void CategoriaDTO_NombreWithMaxLength()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();
            var longNombre = new string('a', 100);

            // Act
            categoriaDTO.Nombre = longNombre;

            // Assert
            Assert.AreEqual(100, categoriaDTO.Nombre.Length);
        }

        [TestMethod]
        public void CategoriaDTO_DescripcionWithMaxLength()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO();
            var longDescripcion = new string('a', 250);

            // Act
            categoriaDTO.Descripcion = longDescripcion;

            // Assert
            Assert.AreEqual(250, categoriaDTO.Descripcion.Length);
        }

        [TestMethod]
        public void CategoriaDTO_EmptyStringsAreValid()
        {
            // Arrange & Act
            var categoriaDTO = new CategoriaDTO
            {
                Nombre = string.Empty,
                Descripcion = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, categoriaDTO.Nombre);
            Assert.AreEqual(string.Empty, categoriaDTO.Descripcion);
        }

        #endregion

        #region Equality Tests

        [TestMethod]
        public void CategoriaDTO_SameValues_HaveSameState()
        {
            // Arrange
            var dto1 = new CategoriaDTO { Id = 1, Nombre = "Test" };
            var dto2 = new CategoriaDTO { Id = 1, Nombre = "Test" };

            // Assert
            Assert.AreEqual(dto1.Id, dto2.Id);
            Assert.AreEqual(dto1.Nombre, dto2.Nombre);
        }

        #endregion
    }
}
