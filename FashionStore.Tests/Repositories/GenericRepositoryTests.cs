using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Tests.Repositories
{
    [TestClass]
    public class GenericRepositoryTests
    {
        private FashionStoreDbContext _context = null!;
        private GenericRepository<Categoria> _categoriaRepository = null!;
        private GenericRepository<Prenda> _prendaRepository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FashionStoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"FashionStoreDb_{Guid.NewGuid()}")
                .Options;

            _context = new FashionStoreDbContext(options);
            _categoriaRepository = new GenericRepository<Categoria>(_context);
            _prendaRepository = new GenericRepository<Prenda>(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _categoriaRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_WithMultipleCategories_ReturnsAllCategories()
        {
            // Arrange
            var categoria1 = new Categoria { Nombre = "Camisetas", Descripcion = "Camisetas variadas" };
            var categoria2 = new Categoria { Nombre = "Pantalones", Descripcion = "Pantalones variados" };
            var categoria3 = new Categoria { Nombre = "Zapatos", Descripcion = "Zapatos variados" };

            await _categoriaRepository.AddAsync(categoria1);
            await _categoriaRepository.AddAsync(categoria2);
            await _categoriaRepository.AddAsync(categoria3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaRepository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_WithDifferentEntityTypes_ReturnsDifferentResults()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Ropa" };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var prenda = new Prenda
            {
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = 1
            };
            await _prendaRepository.AddAsync(prenda);
            await _context.SaveChangesAsync();

            // Act
            var categorias = await _categoriaRepository.GetAllAsync();
            var prendas = await _prendaRepository.GetAllAsync();

            // Assert
            Assert.AreEqual(1, categorias.Count());
            Assert.AreEqual(1, prendas.Count());
        }

        #endregion

        #region GetByIdAsync Tests

        [TestMethod]
        public async Task GetByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Camisetas" };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var addedCategoria = _context.Set<Categoria>().First();

            // Act
            var result = await _categoriaRepository.GetByIdAsync(addedCategoria.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Camisetas", result.Nombre);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _categoriaRepository.GetByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithMultipleEntities_ReturnsCorrectOne()
        {
            // Arrange
            var categoria1 = new Categoria { Nombre = "Categoría 1" };
            var categoria2 = new Categoria { Nombre = "Categoría 2" };
            var categoria3 = new Categoria { Nombre = "Categoría 3" };

            await _categoriaRepository.AddAsync(categoria1);
            await _categoriaRepository.AddAsync(categoria2);
            await _categoriaRepository.AddAsync(categoria3);
            await _context.SaveChangesAsync();

            var allCategorias = _context.Set<Categoria>().ToList();
            var targetId = allCategorias[1].Id;

            // Act
            var result = await _categoriaRepository.GetByIdAsync(targetId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Categoría 2", result.Nombre);
        }

        #endregion

        #region AddAsync Tests

        [TestMethod]
        public async Task AddAsync_WithValidCategory_AddsToDatabase()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Zapatos", Descripcion = "Zapatos de calidad" };

            // Act
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetAllAsync();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Zapatos", result.First().Nombre);
        }

        [TestMethod]
        public async Task AddAsync_WithMultipleCategories_AddsAllToDatabase()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Nombre = "Cat1" },
                new Categoria { Nombre = "Cat2" },
                new Categoria { Nombre = "Cat3" }
            };

            // Act
            foreach (var cat in categorias)
            {
                await _categoriaRepository.AddAsync(cat);
            }
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetAllAsync();
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task AddAsync_WithPrenda_AddsWithCorrectProperties()
        {
            // Arrange
            var prenda = new Prenda
            {
                Nombre = "Pantalón Azul",
                Descripcion = "Pantalón de tela resistente",
                Talla = "32",
                Color = "Azul",
                Precio = 49.99m,
                Stock = 100,
                CategoriaId = 1
            };

            // Act
            await _prendaRepository.AddAsync(prenda);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _prendaRepository.GetAllAsync();
            Assert.AreEqual(1, result.Count());
            var addedPrenda = result.First();
            Assert.AreEqual("Pantalón Azul", addedPrenda.Nombre);
            Assert.AreEqual(49.99m, addedPrenda.Precio);
            Assert.AreEqual(100, addedPrenda.Stock);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public async Task Update_WithValidCategory_UpdatesDatabase()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Original" };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var addedCategoria = _context.Set<Categoria>().First();
            addedCategoria.Nombre = "Actualizada";

            // Act
            _categoriaRepository.Update(addedCategoria);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetByIdAsync(addedCategoria.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Actualizada", result.Nombre);
        }

        [TestMethod]
        public async Task Update_WithMultipleProperties_UpdatesAll()
        {
            // Arrange
            var prenda = new Prenda
            {
                Nombre = "Camiseta Original",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };
            await _prendaRepository.AddAsync(prenda);
            await _context.SaveChangesAsync();

            var addedPrenda = _context.Set<Prenda>().First();
            addedPrenda.Nombre = "Camiseta Actualizada";
            addedPrenda.Precio = 24.99m;
            addedPrenda.Stock = 30;

            // Act
            _prendaRepository.Update(addedPrenda);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _prendaRepository.GetByIdAsync(addedPrenda.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Camiseta Actualizada", result.Nombre);
            Assert.AreEqual(24.99m, result.Precio);
            Assert.AreEqual(30, result.Stock);
        }

        [TestMethod]
        public async Task Update_WithNullableProperties_UpdatesCorrectly()
        {
            // Arrange
            var categoria = new Categoria 
            { 
                Nombre = "Categoría", 
                Descripcion = "Descripción original" 
            };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var addedCategoria = _context.Set<Categoria>().First();
            addedCategoria.Descripcion = null;

            // Act
            _categoriaRepository.Update(addedCategoria);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetByIdAsync(addedCategoria.Id);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Descripcion);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public async Task Delete_WithValidCategory_RemovesFromDatabase()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "A Eliminar" };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var addedCategoria = _context.Set<Categoria>().First();

            // Act
            _categoriaRepository.Delete(addedCategoria);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetByIdAsync(addedCategoria.Id);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Delete_WithMultipleCategories_DeletesOnlyTarget()
        {
            // Arrange
            var categoria1 = new Categoria { Nombre = "Cat1" };
            var categoria2 = new Categoria { Nombre = "Cat2" };
            var categoria3 = new Categoria { Nombre = "Cat3" };

            await _categoriaRepository.AddAsync(categoria1);
            await _categoriaRepository.AddAsync(categoria2);
            await _categoriaRepository.AddAsync(categoria3);
            await _context.SaveChangesAsync();

            var categorias = _context.Set<Categoria>().ToList();
            var toDelete = categorias[1];

            // Act
            _categoriaRepository.Delete(toDelete);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _categoriaRepository.GetAllAsync();
            Assert.AreEqual(2, result.Count());
            Assert.IsFalse(result.Any(c => c.Nombre == "Cat2"));
        }

        [TestMethod]
        public async Task Delete_WithPrenda_RemovesCorrectly()
        {
            // Arrange
            var prenda = new Prenda
            {
                Nombre = "A eliminar",
                Talla = "L",
                Color = "Negro",
                Precio = 39.99m,
                Stock = 10,
                CategoriaId = 1
            };
            await _prendaRepository.AddAsync(prenda);
            await _context.SaveChangesAsync();

            var addedPrenda = _context.Set<Prenda>().First();

            // Act
            _prendaRepository.Delete(addedPrenda);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _prendaRepository.GetAllAsync();
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region FindAsync Tests

        [TestMethod]
        public async Task FindAsync_WithValidExpression_ReturnsMatchingCategories()
        {
            // Arrange
            var categoria1 = new Categoria { Nombre = "Camisetas" };
            var categoria2 = new Categoria { Nombre = "Pantalones" };
            var categoria3 = new Categoria { Nombre = "Camisetas Deportivas" };

            await _categoriaRepository.AddAsync(categoria1);
            await _categoriaRepository.AddAsync(categoria2);
            await _categoriaRepository.AddAsync(categoria3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaRepository.FindAsync(c => c.Nombre.Contains("Camisetas"));

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(c => c.Nombre.Contains("Camisetas")));
        }

        [TestMethod]
        public async Task FindAsync_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Camisetas" };
            await _categoriaRepository.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaRepository.FindAsync(c => c.Nombre == "NoExiste");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task FindAsync_WithComplexExpression_ReturnsCorrectResults()
        {
            // Arrange
            var prenda1 = new Prenda
            {
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };
            var prenda2 = new Prenda
            {
                Nombre = "Pantalón",
                Talla = "32",
                Color = "Azul",
                Precio = 49.99m,
                Stock = 30,
                CategoriaId = 2
            };
            var prenda3 = new Prenda
            {
                Nombre = "Zapato",
                Talla = "42",
                Color = "Negro",
                Precio = 79.99m,
                Stock = 20,
                CategoriaId = 3
            };

            await _prendaRepository.AddAsync(prenda1);
            await _prendaRepository.AddAsync(prenda2);
            await _prendaRepository.AddAsync(prenda3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _prendaRepository.FindAsync(p => p.Precio > 30 && p.Stock < 50);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(p => p.Precio > 30 && p.Stock < 50));
        }

        [TestMethod]
        public async Task FindAsync_WithMultipleCriteria_ReturnsExactMatch()
        {
            // Arrange
            var prenda1 = new Prenda
            {
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };
            var prenda2 = new Prenda
            {
                Nombre = "Camiseta",
                Talla = "L",
                Color = "Azul",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            await _prendaRepository.AddAsync(prenda1);
            await _prendaRepository.AddAsync(prenda2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _prendaRepository.FindAsync(p => p.Color == "Rojo" && p.Talla == "M");

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Rojo", result.First().Color);
            Assert.AreEqual("M", result.First().Talla);
        }

        #endregion
    }
}
