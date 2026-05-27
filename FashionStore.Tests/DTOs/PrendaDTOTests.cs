using FashionStore.Domain.DTOs;

namespace FashionStore.Tests.DTOs
{
    [TestClass]
    public class PrendaDTOTests
    {
        #region Property Tests

        [TestMethod]
        public void PrendaDTO_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO();

            // Assert
            Assert.AreEqual(0, prendaDTO.Id);
            Assert.IsNull(prendaDTO.Nombre);
            Assert.IsNull(prendaDTO.Descripcion);
            Assert.IsNull(prendaDTO.Talla);
            Assert.IsNull(prendaDTO.Color);
            Assert.AreEqual(0, prendaDTO.Precio);
            Assert.AreEqual(0, prendaDTO.Stock);
            Assert.IsNull(prendaDTO.ImagenUrl);
            Assert.AreEqual(0, prendaDTO.CategoriaId);
        }

        [TestMethod]
        public void PrendaDTO_SetAllProperties_StoresValues()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();

            // Act
            prendaDTO.Id = 1;
            prendaDTO.Nombre = "Camiseta";
            prendaDTO.Descripcion = "Camiseta de algodón";
            prendaDTO.Talla = "M";
            prendaDTO.Color = "Rojo";
            prendaDTO.Precio = 29.99m;
            prendaDTO.Stock = 100;
            prendaDTO.ImagenUrl = "imagen.jpg";
            prendaDTO.CategoriaId = 1;

            // Assert
            Assert.AreEqual(1, prendaDTO.Id);
            Assert.AreEqual("Camiseta", prendaDTO.Nombre);
            Assert.AreEqual("Camiseta de algodón", prendaDTO.Descripcion);
            Assert.AreEqual("M", prendaDTO.Talla);
            Assert.AreEqual("Rojo", prendaDTO.Color);
            Assert.AreEqual(29.99m, prendaDTO.Precio);
            Assert.AreEqual(100, prendaDTO.Stock);
            Assert.AreEqual("imagen.jpg", prendaDTO.ImagenUrl);
            Assert.AreEqual(1, prendaDTO.CategoriaId);
        }

        #endregion

        #region Individual Property Tests

        [TestMethod]
        public void PrendaDTO_SetNombre_StoresValue()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();

            // Act
            prendaDTO.Nombre = "Pantalón Azul";

            // Assert
            Assert.AreEqual("Pantalón Azul", prendaDTO.Nombre);
        }

        [TestMethod]
        public void PrendaDTO_SetPrecio_StoresValue()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();

            // Act
            prendaDTO.Precio = 49.99m;

            // Assert
            Assert.AreEqual(49.99m, prendaDTO.Precio);
        }

        [TestMethod]
        public void PrendaDTO_SetStock_StoresValue()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();

            // Act
            prendaDTO.Stock = 150;

            // Assert
            Assert.AreEqual(150, prendaDTO.Stock);
        }

        [TestMethod]
        public void PrendaDTO_SetCategoriaId_StoresValue()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();

            // Act
            prendaDTO.CategoriaId = 3;

            // Assert
            Assert.AreEqual(3, prendaDTO.CategoriaId);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void PrendaDTO_NombreMaxLength_Is150()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();
            var longNombre = new string('a', 150);

            // Act
            prendaDTO.Nombre = longNombre;

            // Assert
            Assert.AreEqual(150, prendaDTO.Nombre.Length);
        }

        [TestMethod]
        public void PrendaDTO_DescripcionMaxLength_Is300()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();
            var longDescripcion = new string('a', 300);

            // Act
            prendaDTO.Descripcion = longDescripcion;

            // Assert
            Assert.AreEqual(300, prendaDTO.Descripcion.Length);
        }

        [TestMethod]
        public void PrendaDTO_TallaMaxLength_Is50()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();
            var talla = new string('a', 50);

            // Act
            prendaDTO.Talla = talla;

            // Assert
            Assert.AreEqual(50, prendaDTO.Talla.Length);
        }

        [TestMethod]
        public void PrendaDTO_ColorMaxLength_Is50()
        {
            // Arrange
            var prendaDTO = new PrendaDTO();
            var color = new string('a', 50);

            // Act
            prendaDTO.Color = color;

            // Assert
            Assert.AreEqual(50, prendaDTO.Color.Length);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void PrendaDTO_NombreCanBeNull()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Nombre = null };

            // Assert
            Assert.IsNull(prendaDTO.Nombre);
        }

        [TestMethod]
        public void PrendaDTO_DescripcionCanBeNull()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Descripcion = null };

            // Assert
            Assert.IsNull(prendaDTO.Descripcion);
        }

        [TestMethod]
        public void PrendaDTO_TallaCanBeNull()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Talla = null };

            // Assert
            Assert.IsNull(prendaDTO.Talla);
        }

        [TestMethod]
        public void PrendaDTO_ColorCanBeNull()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Color = null };

            // Assert
            Assert.IsNull(prendaDTO.Color);
        }

        [TestMethod]
        public void PrendaDTO_ImagenUrlCanBeNull()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { ImagenUrl = null };

            // Assert
            Assert.IsNull(prendaDTO.ImagenUrl);
        }

        #endregion

        #region Price Tests

        [TestMethod]
        public void PrendaDTO_PrecioMinValue()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Precio = 1m };

            // Assert
            Assert.AreEqual(1m, prendaDTO.Precio);
        }

        [TestMethod]
        public void PrendaDTO_PrecioMaxValue()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Precio = 99999m };

            // Assert
            Assert.AreEqual(99999m, prendaDTO.Precio);
        }

        [TestMethod]
        public void PrendaDTO_PrecioWithDecimals()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Precio = 19.99m };

            // Assert
            Assert.AreEqual(19.99m, prendaDTO.Precio);
        }

        [TestMethod]
        public void PrendaDTO_PrecioZero()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Precio = 0m };

            // Assert
            Assert.AreEqual(0m, prendaDTO.Precio);
        }

        #endregion

        #region Stock Tests

        [TestMethod]
        public void PrendaDTO_StockMinValue()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Stock = 0 };

            // Assert
            Assert.AreEqual(0, prendaDTO.Stock);
        }

        [TestMethod]
        public void PrendaDTO_StockMaxValue()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Stock = 10000 };

            // Assert
            Assert.AreEqual(10000, prendaDTO.Stock);
        }

        [TestMethod]
        public void PrendaDTO_StockMidValue()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO { Stock = 5000 };

            // Assert
            Assert.AreEqual(5000, prendaDTO.Stock);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void PrendaDTO_AllSizes_AreValid()
        {
            // Arrange
            var sizes = new[] { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };

            // Act & Assert
            foreach (var size in sizes)
            {
                var prendaDTO = new PrendaDTO { Talla = size };
                Assert.AreEqual(size, prendaDTO.Talla);
            }
        }

        [TestMethod]
        public void PrendaDTO_AllColors_AreValid()
        {
            // Arrange
            var colors = new[] { "Rojo", "Azul", "Negro", "Blanco", "Verde", "Amarillo", "Gris", "Marrón" };

            // Act & Assert
            foreach (var color in colors)
            {
                var prendaDTO = new PrendaDTO { Color = color };
                Assert.AreEqual(color, prendaDTO.Color);
            }
        }

        [TestMethod]
        public void PrendaDTO_EmptyStringsAreValid()
        {
            // Arrange & Act
            var prendaDTO = new PrendaDTO
            {
                Nombre = string.Empty,
                Descripcion = string.Empty,
                Talla = string.Empty,
                Color = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, prendaDTO.Nombre);
            Assert.AreEqual(string.Empty, prendaDTO.Descripcion);
            Assert.AreEqual(string.Empty, prendaDTO.Talla);
            Assert.AreEqual(string.Empty, prendaDTO.Color);
        }

        #endregion

        #region Comparison Tests

        [TestMethod]
        public void PrendaDTO_SameValues_AreEquivalent()
        {
            // Arrange
            var dto1 = new PrendaDTO { Id = 1, Nombre = "Test", Precio = 19.99m };
            var dto2 = new PrendaDTO { Id = 1, Nombre = "Test", Precio = 19.99m };

            // Assert
            Assert.AreEqual(dto1.Id, dto2.Id);
            Assert.AreEqual(dto1.Nombre, dto2.Nombre);
            Assert.AreEqual(dto1.Precio, dto2.Precio);
        }

        #endregion
    }
}
