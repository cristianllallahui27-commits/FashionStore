using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FashionStore.Tests.Controllers
{
    [TestClass]
    public class CategoriasControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<IMapper> _mockMapper = null!;
        private CategoriasController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CategoriasController(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region Index Tests

        [TestMethod]
        public async Task Index_WithCategories_ReturnsViewWithCategories()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" },
                new Categoria { Id = 2, Nombre = "Pantalones" }
            };

            var categoriasDTO = new List<CategoriaDTO>
            {
                new CategoriaDTO { Id = 1, Nombre = "Camisetas" },
                new CategoriaDTO { Id = 2, Nombre = "Pantalones" }
            };

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDTO>>(It.IsAny<IEnumerable<Categoria>>()))
                .Returns(categoriasDTO);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult?.Model);
            Assert.AreEqual(categoriasDTO, viewResult.Model);
            _mockUnitOfWork.Verify(u => u.Categorias.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Index_WithNoCategories_ReturnsEmptyList()
        {
            // Arrange
            var categorias = new List<Categoria>();
            var categoriasDTO = new List<CategoriaDTO>();

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDTO>>(It.IsAny<IEnumerable<Categoria>>()))
                .Returns(categoriasDTO);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as IEnumerable<CategoriaDTO>;
            Assert.AreEqual(0, model?.Count());
        }

        #endregion

        #region Details Tests

        [TestMethod]
        public async Task Details_WithValidId_ReturnsCategory()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Camisetas" };
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Camisetas" };

            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(1))
                .ReturnsAsync(categoria);

            _mockMapper.Setup(m => m.Map<CategoriaDTO>(It.IsAny<Categoria>()))
                .Returns(categoriaDTO);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(categoriaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(999))
                .ReturnsAsync((Categoria?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Create GET Tests

        [TestMethod]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        #endregion

        #region Create POST Tests

        [TestMethod]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Nueva Categoría" };
            var categoria = new Categoria { Id = 1, Nombre = "Nueva Categoría" };

            _mockMapper.Setup(m => m.Map<Categoria>(It.IsAny<CategoriaDTO>()))
                .Returns(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.AddAsync(It.IsAny<Categoria>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(CategoriasController.Index), redirectResult?.ActionName);
            _mockUnitOfWork.Verify(u => u.Categorias.AddAsync(It.IsAny<Categoria>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Nombre = string.Empty };
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            // Act
            var result = await _controller.Create(categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_Post_WithValidData_SavesToDatabase()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Categoría" };
            var categoria = new Categoria { Id = 1, Nombre = "Categoría" };

            _mockMapper.Setup(m => m.Map<Categoria>(categoriaDTO))
                .Returns(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.AddAsync(categoria))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            await _controller.Create(categoriaDTO);

            // Assert
            _mockUnitOfWork.Verify(u => u.Categorias.AddAsync(It.Is<Categoria>(c => c.Nombre == "Categoría")), Times.Once);
        }

        #endregion

        #region Edit GET Tests

        [TestMethod]
        public async Task Edit_Get_WithValidId_ReturnsCategoryDTO()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Camisetas" };
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Camisetas" };

            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(1))
                .ReturnsAsync(categoria);

            _mockMapper.Setup(m => m.Map<CategoriaDTO>(categoria))
                .Returns(categoriaDTO);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(categoriaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(999))
                .ReturnsAsync((Categoria?)null);

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
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Categoría Actualizada" };
            var categoria = new Categoria { Id = 1, Nombre = "Categoría Actualizada" };

            _mockMapper.Setup(m => m.Map<Categoria>(categoriaDTO))
                .Returns(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.Update(It.IsAny<Categoria>()))
                .Callback<Categoria>(c => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(1, categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            _mockUnitOfWork.Verify(u => u.Categorias.Update(It.IsAny<Categoria>()), Times.Once);
        }

        [TestMethod]
        public async Task Edit_Post_WithMismatchedId_ReturnsNotFound()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Categoría" };

            // Act
            var result = await _controller.Edit(2, categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = string.Empty };
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            // Act
            var result = await _controller.Edit(1, categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        #endregion

        #region Delete GET Tests

        [TestMethod]
        public async Task Delete_Get_WithValidId_ReturnsCategoryDTO()
        {
            // Arrange
            var categoria = new Categoria { Id = 1, Nombre = "Camisetas" };
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Camisetas" };

            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(1))
                .ReturnsAsync(categoria);

            _mockMapper.Setup(m => m.Map<CategoriaDTO>(categoria))
                .Returns(categoriaDTO);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(categoriaDTO, viewResult?.Model);
        }

        [TestMethod]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(999))
                .ReturnsAsync((Categoria?)null);

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
            var categoria = new Categoria { Id = 1, Nombre = "Camisetas" };

            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(1))
                .ReturnsAsync(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.Delete(It.IsAny<Categoria>()))
                .Callback<Categoria>(c => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            _mockUnitOfWork.Verify(u => u.Categorias.Delete(It.IsAny<Categoria>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteConfirmed_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Categorias.GetByIdAsync(999))
                .ReturnsAsync((Categoria?)null);

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Edge Case Tests

        [TestMethod]
        public async Task Create_Post_WithDescriptionOnly_CreatesSuccessfully()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Cat", Descripcion = "Descripción" };
            var categoria = new Categoria { Id = 1, Nombre = "Cat", Descripcion = "Descripción" };

            _mockMapper.Setup(m => m.Map<Categoria>(categoriaDTO))
                .Returns(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.AddAsync(It.IsAny<Categoria>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_Post_WithNullDescription_UpdatesSuccessfully()
        {
            // Arrange
            var categoriaDTO = new CategoriaDTO { Id = 1, Nombre = "Cat", Descripcion = null };
            var categoria = new Categoria { Id = 1, Nombre = "Cat", Descripcion = null };

            _mockMapper.Setup(m => m.Map<Categoria>(categoriaDTO))
                .Returns(categoria);

            _mockUnitOfWork.Setup(u => u.Categorias.Update(It.IsAny<Categoria>()))
                .Callback<Categoria>(c => { });

            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(1, categoriaDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        #endregion
    }
}
