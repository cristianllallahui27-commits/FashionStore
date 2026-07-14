using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Tests.UnitOfWork
{
    [TestClass]
    public class UnitOfWorkTests
    {
        private FashionStoreDbContext _context = null!;
        private Infrastructure.UnitOfWork.UnitOfWork _unitOfWork = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FashionStoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"FashionStoreDb_{Guid.NewGuid()}")
                .Options;

            _context = new FashionStoreDbContext(options);
            _unitOfWork = new Infrastructure.UnitOfWork.UnitOfWork(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _unitOfWork?.Dispose();
            _context?.Dispose();
        }

        #region Repository Access Tests

        [TestMethod]
        public void Categorias_Property_IsNotNull() =>
            // Assert
            Assert.IsNotNull(_unitOfWork.Categorias);

        [TestMethod]
        public void Prendas_Property_IsNotNull() =>
            // Assert
            Assert.IsNotNull(_unitOfWork.Prendas);

        [TestMethod]
        public void Categorias_Property_ReturnsGenericRepository()
        {
            // Assert
            Assert.IsNotNull(_unitOfWork.Categorias);
            Assert.IsInstanceOfType(_unitOfWork.Categorias, typeof(Infrastructure.Repositories.GenericRepository<Categoria>));
        }

        [TestMethod]
        public void Prendas_Property_ReturnsGenericRepository()
        {
            // Assert
            Assert.IsNotNull(_unitOfWork.Prendas);
            Assert.IsInstanceOfType(_unitOfWork.Prendas, typeof(Infrastructure.Repositories.GenericRepository<Prenda>));
        }

        #endregion

        #region CommitAsync Tests

        [TestMethod]
        public async Task CommitAsync_WithNoChanges_ReturnsZero()
        {
            // Act
            var result = await _unitOfWork.CommitAsync();

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task CommitAsync_WithAddedCategory_ReturnsSaveCount()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Nueva Categoría" };
            await _unitOfWork.Categorias.AddAsync(categoria);

            // Act
            var result = await _unitOfWork.CommitAsync();

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public async Task CommitAsync_WithMultipleChanges_SavesAllChanges()
        {
            // Arrange
            var categoria1 = new Categoria { Nombre = "Cat1" };
            var categoria2 = new Categoria { Nombre = "Cat2" };
            var prenda = new Prenda
            {
                Nombre = "Prenda",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            await _unitOfWork.Categorias.AddAsync(categoria1);
            await _unitOfWork.Categorias.AddAsync(categoria2);
            await _unitOfWork.Prendas.AddAsync(prenda);

            // Act
            var result = await _unitOfWork.CommitAsync();
            var categorias = await _unitOfWork.Categorias.GetAllAsync();
            var prendas = await _unitOfWork.Prendas.GetAllAsync();

            // Assert
            Assert.IsTrue(result > 0);
            Assert.AreEqual(2, categorias.Count());
            Assert.AreEqual(1, prendas.Count());
        }

        [TestMethod]
        public async Task CommitAsync_WithUpdatedCategory_PersistsChanges()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "Original" };
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.CommitAsync();

            var addedCategoria = _context.Set<Categoria>().First();
            addedCategoria.Nombre = "Actualizada";

            // Act
            _unitOfWork.Categorias.Update(addedCategoria);
            var result = await _unitOfWork.CommitAsync();

            // Assert
            Assert.IsTrue(result > 0);
            var updated = await _unitOfWork.Categorias.GetByIdAsync(addedCategoria.Id);
            Assert.AreEqual("Actualizada", updated?.Nombre);
        }

        [TestMethod]
        public async Task CommitAsync_WithDeletedCategory_RemovesFromDatabase()
        {
            // Arrange
            var categoria = new Categoria { Nombre = "A Eliminar" };
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.CommitAsync();

            var addedCategoria = _context.Set<Categoria>().First();

            // Act
            _unitOfWork.Categorias.Delete(addedCategoria);
            var result = await _unitOfWork.CommitAsync();

            // Assert
            Assert.IsTrue(result > 0);
            var deleted = await _unitOfWork.Categorias.GetByIdAsync(addedCategoria.Id);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public async Task CommitAsync_WithMixedOperations_ProcessesAllCorrectly()
        {
            // Arrange
            // Agregar
            var categoria1 = new Categoria { Nombre = "Nueva" };
            await _unitOfWork.Categorias.AddAsync(categoria1);
            await _unitOfWork.CommitAsync();

            var addedCat1 = _context.Set<Categoria>().First();

            // Actualizar
            addedCat1.Nombre = "Actualizada";
            _unitOfWork.Categorias.Update(addedCat1);

            // Agregar otra
            var categoria2 = new Categoria { Nombre = "Otra" };
            await _unitOfWork.Categorias.AddAsync(categoria2);

            // Act
            var result = await _unitOfWork.CommitAsync();
            var all = await _unitOfWork.Categorias.GetAllAsync();

            // Assert
            Assert.IsTrue(result > 0);
            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.Any(c => c.Nombre == "Actualizada"));
            Assert.IsTrue(all.Any(c => c.Nombre == "Otra"));
        }

        #endregion

        #region Dispose Tests

        [TestMethod]
        public void Dispose_WithActiveConnection_DisposesContext() =>
            // Act & Assert - should not throw
            _unitOfWork.Dispose();

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert - should not throw
            _unitOfWork.Dispose();
            _unitOfWork.Dispose();
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public async Task FullWorkflow_AddUpdateDelete_WorksCorrectly()
        {
            // Arrange & Act
            // 1. Agregar categoría
            var categoria = new Categoria { Nombre = "Ropa", Descripcion = "Ropa variada" };
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.CommitAsync();

            var addedCategoria = _context.Set<Categoria>().First();
            var originalId = addedCategoria.Id;

            // 2. Obtener categoría
            var retrieved = await _unitOfWork.Categorias.GetByIdAsync(originalId);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual("Ropa", retrieved.Nombre);

            // 3. Actualizar
            addedCategoria.Descripcion = "Descripción actualizada";
            _unitOfWork.Categorias.Update(addedCategoria);
            await _unitOfWork.CommitAsync();

            var updated = await _unitOfWork.Categorias.GetByIdAsync(originalId);
            Assert.AreEqual("Descripción actualizada", updated?.Descripcion);

            // 4. Eliminar
            _unitOfWork.Categorias.Delete(addedCategoria);
            await _unitOfWork.CommitAsync();

            var deleted = await _unitOfWork.Categorias.GetByIdAsync(originalId);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public async Task MultipleRepositories_WorkIndependently()
        {
            // Arrange & Act
            var categoria = new Categoria { Nombre = "Categoría 1" };
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.CommitAsync();

            var addedCat = _context.Set<Categoria>().First();

            var prenda = new Prenda
            {
                Nombre = "Prenda 1",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = addedCat.Id
            };
            await _unitOfWork.Prendas.AddAsync(prenda);
            await _unitOfWork.CommitAsync();

            // Assert
            var categories = await _unitOfWork.Categorias.GetAllAsync();
            var clothes = await _unitOfWork.Prendas.GetAllAsync();

            Assert.AreEqual(1, categories.Count());
            Assert.AreEqual(1, clothes.Count());
        }

        #endregion
    }
}
