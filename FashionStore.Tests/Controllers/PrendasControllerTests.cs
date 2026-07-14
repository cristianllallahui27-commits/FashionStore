using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace FashionStore.Tests.Controllers
{
    [TestClass]
    public class PrendasControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<IWebHostEnvironment> _mockEnv = null!;
        private PrendasController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            _controller = new PrendasController(_mockUnitOfWork.Object, _mockMapper.Object, _mockEnv.Object);

            // TempData requerido para que TempData["Success"] no lance NullReferenceException
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_WithPrendas_ReturnsViewWithPrendas()
        {
            // Arrange
            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta", Talla = "M", Color = "Rojo", Precio = 19.99m, Stock = 50, CategoriaId = 1 },
                new Prenda { Id = 2, Nombre = "Pantal�n", Talla = "32", Color = "Azul", Precio = 49.99m, Stock = 30, CategoriaId = 2 }
            };

            var prendasDTO = new List<PrendaDTO>
            {
                new PrendaDTO { Id = 1, Nombre = "Camiseta", Talla = "M", Color = "Rojo", Precio = 19.99m, Stock = 50 },
                new PrendaDTO { Id = 2, Nombre = "Pantal�n", Talla = "32", Color = "Azul", Precio = 49.99m, Stock = 30 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockMapper.Setup(m => m.Map<IEnumerable<PrendaDTO>>(It.IsAny<IEnumerable<Prenda>>()))
                .Returns(prendasDTO);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult?.Model);
            Assert.AreEqual(prendasDTO, viewResult.Model);
        }

        [TestMethod]
        public async Task Index_WithNoPrendas_ReturnsEmptyList()
        {
            // Arrange
            var prendas = new List<Prenda>();
            var prendasDTO = new List<PrendaDTO>();

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockMapper.Setup(m => m.Map<IEnumerable<PrendaDTO>>(It.IsAny<IEnumerable<Prenda>>()))
                .Returns(prendasDTO);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_WithValidId_ReturnsPrenda()
        {
            // Arrange
            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(1))
                .ReturnsAsync(prenda);

            _mockMapper.Setup(m => m.Map<PrendaDTO>(It.IsAny<Prenda>()))
                .Returns(prendaDTO);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(prendaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(999))
                .ReturnsAsync((Prenda?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Create GET Tests

        [TestMethod]
        public async Task Create_Get_ReturnsCategoriesInViewBag()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" },
                new Categoria { Id = 2, Nombre = "Pantalones" }
            };

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        #endregion

        #region Create POST Tests

        [TestMethod]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Nueva Prenda",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Nueva Prenda",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = 1
            };

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.AddAsync(It.IsAny<Prenda>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(prendaDTO, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(PrendasController.Index), redirectResult?.ActionName);
        }

        [TestMethod]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var prendaDTO = new PrendaDTO { Nombre = string.Empty };
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            var categorias = new List<Categoria>();
            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Create(prendaDTO, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_Post_WithImageFile_SavesImageUrl()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Prenda con imagen",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Prenda con imagen",
                Talla = "M",
                Color = "Rojo",
                Precio = 29.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.AddAsync(It.IsAny<Prenda>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(prendaDTO, mockFile.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        #endregion

        #region Edit GET Tests

        [TestMethod]
        public async Task Edit_Get_WithValidId_ReturnsPrendaDTO()
        {
            // Arrange
            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(1))
                .ReturnsAsync(prenda);

            _mockMapper.Setup(m => m.Map<PrendaDTO>(It.IsAny<Prenda>()))
                .Returns(prendaDTO);

            var categorias = new List<Categoria>();
            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(prendaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(999))
                .ReturnsAsync((Prenda?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Edit POST Tests

        [TestMethod]
        public async Task Edit_Post_WithValidData_UpdatesAndRedirects()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Prenda Actualizada",
                Talla = "L",
                Color = "Azul",
                Precio = 34.99m,
                Stock = 25,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Prenda Actualizada",
                Talla = "L",
                Color = "Azul",
                Precio = 34.99m,
                Stock = 25,
                CategoriaId = 1
            };

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.Update(It.IsAny<Prenda>()))
                .Callback<Prenda>(p => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(1, prendaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_Post_WithMismatchedId_ReturnsNotFound()
        {
            // Arrange
            var prendaDTO = new PrendaDTO { Id = 1, Nombre = "Prenda" };

            // Act
            var result = await _controller.Edit(2, prendaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var prendaDTO = new PrendaDTO { Id = 1, Nombre = string.Empty };
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            var categorias = new List<Categoria>();
            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Edit(1, prendaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        #endregion

        #region Delete GET Tests

        [TestMethod]
        public async Task Delete_Get_WithValidId_ReturnsPrendaDTO()
        {
            // Arrange
            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(1))
                .ReturnsAsync(prenda);

            _mockMapper.Setup(m => m.Map<PrendaDTO>(It.IsAny<Prenda>()))
                .Returns(prendaDTO);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(prendaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(999))
                .ReturnsAsync((Prenda?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Delete POST Tests

        [TestMethod]
        public async Task DeleteConfirmed_WithValidId_DeletesAndRedirects()
        {
            // Arrange
            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Camiseta",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 50,
                CategoriaId = 1
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(1))
                .ReturnsAsync(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.Delete(It.IsAny<Prenda>()))
                .Callback<Prenda>(p => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            _mockUnitOfWork.Verify(u => u.Prendas.Delete(It.IsAny<Prenda>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteConfirmed_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Prendas.GetByIdAsync(999))
                .ReturnsAsync((Prenda?)null);

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Edge Case Tests

        [TestMethod]
        public async Task Create_Post_WithBoundaryValues_CreatesSuccessfully()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "P",
                Talla = "XS",
                Color = "Rojo",
                Precio = 0.01m,
                Stock = 0,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "P",
                Talla = "XS",
                Color = "Rojo",
                Precio = 0.01m,
                Stock = 0,
                CategoriaId = 1
            };

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.AddAsync(It.IsAny<Prenda>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(prendaDTO, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Create_Post_WithMaxValues_CreatesSuccessfully()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = new string('A', 150),
                Talla = "XXXL",
                Color = "Rojo",
                Precio = 99999m,
                Stock = 10000,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = new string('A', 150),
                Talla = "XXXL",
                Color = "Rojo",
                Precio = 99999m,
                Stock = 10000,
                CategoriaId = 1
            };

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.AddAsync(It.IsAny<Prenda>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(prendaDTO, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_Post_WithZeroStock_UpdatesSuccessfully()
        {
            // Arrange
            var prendaDTO = new PrendaDTO
            {
                Id = 1,
                Nombre = "Prenda",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 0,
                CategoriaId = 1
            };

            var prenda = new Prenda
            {
                Id = 1,
                Nombre = "Prenda",
                Talla = "M",
                Color = "Rojo",
                Precio = 19.99m,
                Stock = 0,
                CategoriaId = 1
            };

            _mockMapper.Setup(m => m.Map<Prenda>(It.IsAny<PrendaDTO>()))
                .Returns(prenda);

            _mockUnitOfWork.Setup(u => u.Prendas.Update(It.IsAny<Prenda>()))
                .Callback<Prenda>(p => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(1, prendaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        #endregion
    }
}
