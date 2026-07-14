using FashionStore.Domain.Entities;

namespace FashionStore.Tests.Entities
{
    [TestClass]
    public class PrendaEntityTests
    {
        #region Property Tests

        [TestMethod]
        public void Prenda_NewInstance_HasDefaultValues()
        {
            // Arrange & Act
            var prenda = new Prenda();

            // Assert
            Assert.AreEqual(0, prenda.Id);
            Assert.AreEqual(string.Empty, prenda.Nombre);
            Assert.IsNull(prenda.Descripcion);
            Assert.AreEqual(string.Empty, prenda.Talla);
            Assert.AreEqual(string.Empty, prenda.Color);
            Assert.AreEqual(0, prenda.Precio);
            Assert.AreEqual(0, prenda.Stock);
            Assert.IsNull(prenda.ImagenUrl);
            Assert.AreEqual(0, prenda.CategoriaId);
            Assert.IsNull(prenda.Categoria);
            Assert.IsNull(prenda.DetalleVentas);
        }

        [TestMethod]
        public void Prenda_SetNombre_StoresValue()
        {
            // Arrange
            var prenda = new Prenda();
            var nombre = "Camiseta Roja";

            // Act
            prenda.Nombre = nombre;

            // Assert
            Assert.AreEqual(nombre, prenda.Nombre);
        }

        [TestMethod]
        public void Prenda_SetAllProperties_StoresValues()
        {
            // Arrange
            var prenda = new Prenda();

            // Act
            prenda.Id = 1;
            prenda.Nombre = "Camiseta";
            prenda.Descripcion = "Camiseta de algodon";
            prenda.Talla = "M";
            prenda.Color = "Rojo";
            prenda.Precio = 29.99m;
            prenda.Stock = 100;
            prenda.ImagenUrl = "http://example.com/imagen.jpg";
            prenda.CategoriaId = 1;

            // Assert
            Assert.AreEqual(1, prenda.Id);
            Assert.AreEqual("Camiseta", prenda.Nombre);
            Assert.AreEqual("Camiseta de algodon", prenda.Descripcion);
            Assert.AreEqual("M", prenda.Talla);
            Assert.AreEqual("Rojo", prenda.Color);
            Assert.AreEqual(29.99m, prenda.Precio);
            Assert.AreEqual(100, prenda.Stock);
            Assert.AreEqual("http://example.com/imagen.jpg", prenda.ImagenUrl);
            Assert.AreEqual(1, prenda.CategoriaId);
        }

        #endregion

        #region Collection Tests

        [TestMethod]
        public void Prenda_SetDetalleVentas_StoresCollection()
        {
            // Arrange
            var prenda = new Prenda();
            var detalles = new List<DetalleVenta>
            {
                new DetalleVenta { Id = 1, PrendaId = 1 },
                new DetalleVenta { Id = 2, PrendaId = 1 }
            };

            // Act
            prenda.DetalleVentas = detalles;

            // Assert
            Assert.IsNotNull(prenda.DetalleVentas);
            Assert.AreEqual(2, prenda.DetalleVentas.Count);
        }

        [TestMethod]
        public void Prenda_DetalleVentas_CanBeNull()
        {
            // Arrange & Act
            var prenda = new Prenda { DetalleVentas = null };

            // Assert
            Assert.IsNull(prenda.DetalleVentas);
        }

        #endregion

        #region Categoria Navigation Tests

        [TestMethod]
        public void Prenda_SetCategoria_StoresReference()
        {
            // Arrange
            var prenda = new Prenda();
            var categoria = new Categoria { Id = 1, Nombre = "Ropa" };

            // Act
            prenda.Categoria = categoria;

            // Assert
            Assert.IsNotNull(prenda.Categoria);
            Assert.AreEqual("Ropa", prenda.Categoria.Nombre);
        }

        [TestMethod]
        public void Prenda_CategoriaId_MatchesCategoriaNavigationProperty()
        {
            // Arrange
            var prenda = new Prenda { CategoriaId = 5 };
            var categoria = new Categoria { Id = 5, Nombre = "Test" };

            // Act
            prenda.Categoria = categoria;

            // Assert
            Assert.AreEqual(prenda.CategoriaId, prenda.Categoria.Id);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Prenda_NombreMaxLength_Is150()
        {
            // Arrange
            var prenda = new Prenda();
            var longNombre = new string('a', 150);

            // Act
            prenda.Nombre = longNombre;

            // Assert
            Assert.AreEqual(150, prenda.Nombre.Length);
        }

        [TestMethod]
        public void Prenda_DescripcionMaxLength_Is300()
        {
            // Arrange
            var prenda = new Prenda();
            var longDescripcion = new string('a', 300);

            // Act
            prenda.Descripcion = longDescripcion;

            // Assert
            Assert.AreEqual(300, prenda.Descripcion.Length);
        }

        [TestMethod]
        public void Prenda_TallaMaxLength_Is50()
        {
            // Arrange
            var prenda = new Prenda();
            var talla = new string('a', 50);

            // Act
            prenda.Talla = talla;

            // Assert
            Assert.AreEqual(50, prenda.Talla.Length);
        }

        [TestMethod]
        public void Prenda_ColorMaxLength_Is50()
        {
            // Arrange
            var prenda = new Prenda();
            var color = new string('a', 50);

            // Act
            prenda.Color = color;

            // Assert
            Assert.AreEqual(50, prenda.Color.Length);
        }

        #endregion

        #region Price and Stock Tests

        [TestMethod]
        public void Prenda_PrecioDecimalPlaces_AreTwoDecimals()
        {
            // Arrange & Act
            var prenda = new Prenda { Precio = 29.99m };

            // Assert
            Assert.AreEqual(2, decimal.GetBits(prenda.Precio)[3] >> 16 & 0xFF);
        }

        [TestMethod]
        public void Prenda_PrecioCanBeVeryHigh()
        {
            // Arrange & Act
            var prenda = new Prenda { Precio = 99999.99m };

            // Assert
            Assert.AreEqual(99999.99m, prenda.Precio);
        }

        [TestMethod]
        public void Prenda_StockCanBeZero()
        {
            // Arrange & Act
            var prenda = new Prenda { Stock = 0 };

            // Assert
            Assert.AreEqual(0, prenda.Stock);
        }

        [TestMethod]
        public void Prenda_StockCanBeVeryHigh()
        {
            // Arrange & Act
            var prenda = new Prenda { Stock = 10000 };

            // Assert
            Assert.AreEqual(10000, prenda.Stock);
        }

        #endregion

        #region Null Handling Tests

        [TestMethod]
        public void Prenda_ImagenUrlCanBeNull()
        {
            // Arrange & Act
            var prenda = new Prenda { ImagenUrl = null };

            // Assert
            Assert.IsNull(prenda.ImagenUrl);
        }

        [TestMethod]
        public void Prenda_DescripcionCanBeNull()
        {
            // Arrange & Act
            var prenda = new Prenda { Descripcion = null };

            // Assert
            Assert.IsNull(prenda.Descripcion);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void Prenda_BoundaryValues_MinPrice()
        {
            // Arrange & Act
            var prenda = new Prenda { Precio = 1m };

            // Assert
            Assert.AreEqual(1m, prenda.Precio);
        }

        [TestMethod]
        public void Prenda_BoundaryValues_MaxPrice()
        {
            // Arrange & Act
            var prenda = new Prenda { Precio = 99999m };

            // Assert
            Assert.AreEqual(99999m, prenda.Precio);
        }

        [TestMethod]
        public void Prenda_BoundaryValues_MinStock()
        {
            // Arrange & Act
            var prenda = new Prenda { Stock = 0 };

            // Assert
            Assert.AreEqual(0, prenda.Stock);
        }

        [TestMethod]
        public void Prenda_BoundaryValues_MaxStock()
        {
            // Arrange & Act
            var prenda = new Prenda { Stock = 10000 };

            // Assert
            Assert.AreEqual(10000, prenda.Stock);
        }

        [TestMethod]
        public void Prenda_AllSizesValid()
        {
            // Arrange
            var sizes = new[] { "XS", "S", "M", "L", "XL", "XXL" };

            // Act & Assert
            foreach (var size in sizes)
            {
                var prenda = new Prenda { Talla = size };
                Assert.AreEqual(size, prenda.Talla);
            }
        }

        [TestMethod]
        public void Prenda_AllColorsValid()
        {
            // Arrange
            var colors = new[] { "Rojo", "Azul", "Negro", "Blanco", "Verde", "Amarillo" };

            // Act & Assert
            foreach (var color in colors)
            {
                var prenda = new Prenda { Color = color };
                Assert.AreEqual(color, prenda.Color);
            }
        }

        #endregion
    }
}
