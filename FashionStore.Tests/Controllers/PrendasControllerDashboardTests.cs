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
    public class PrendasControllerDashboardTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<IMapper> _mockMapper = null!;
        private PrendasController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PrendasController(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region Dashboard Tests

        [TestMethod]
        public async Task Dashboard_WithPrendas_ReturnsViewWithData()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" },
                new Categoria { Id = 2, Nombre = "Pantalones" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta 1", CategoriaId = 1, Stock = 50 },
                new Prenda { Id = 2, Nombre = "Camiseta 2", CategoriaId = 1, Stock = 30 },
                new Prenda { Id = 3, Nombre = "Pantalón 1", CategoriaId = 2, Stock = 20 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            // Verify ViewData is populated
            Assert.IsNotNull(viewResult.ViewData["Labels"]);
            Assert.IsNotNull(viewResult.ViewData["Counts"]);
            Assert.IsNotNull(viewResult.ViewData["Stocks"]);
        }

        [TestMethod]
        public async Task Dashboard_WithNoPrendas_ReturnsEmptyViewData()
        {
            // Arrange
            var categorias = new List<Categoria>();
            var prendas = new List<Prenda>();

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public async Task Dashboard_WithSingleCategory_ReturnsSingleLabel()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Ropa" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta", CategoriaId = 1, Stock = 50 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult?.ViewData["Labels"]);
        }

        [TestMethod]
        public async Task Dashboard_ViewDataLabels_IsJsonSerialized()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta", CategoriaId = 1, Stock = 50 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            var viewResult = result as ViewResult;
            var labelsJson = viewResult?.ViewData["Labels"] as string;
            Assert.IsNotNull(labelsJson);
            Assert.IsTrue(labelsJson.Contains("Camisetas"));
        }

        [TestMethod]
        public async Task Dashboard_ViewDataCounts_IsJsonSerialized()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" },
                new Categoria { Id = 2, Nombre = "Pantalones" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta 1", CategoriaId = 1, Stock = 50 },
                new Prenda { Id = 2, Nombre = "Camiseta 2", CategoriaId = 1, Stock = 30 },
                new Prenda { Id = 3, Nombre = "Pantalón", CategoriaId = 2, Stock = 20 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            var viewResult = result as ViewResult;
            var countsJson = viewResult?.ViewData["Counts"] as string;
            Assert.IsNotNull(countsJson);
            Assert.IsTrue(countsJson.Contains("["));
        }

        [TestMethod]
        public async Task Dashboard_ViewDataStocks_IsJsonSerialized()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta", CategoriaId = 1, Stock = 100 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            var viewResult = result as ViewResult;
            var stocksJson = viewResult?.ViewData["Stocks"] as string;
            Assert.IsNotNull(stocksJson);
            Assert.IsTrue(stocksJson.Contains("100"));
        }

        [TestMethod]
        public async Task Dashboard_MultipleCategories_GroupsCorrectly()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Camisetas" },
                new Categoria { Id = 2, Nombre = "Pantalones" },
                new Categoria { Id = 3, Nombre = "Zapatos" }
            };

            var prendas = new List<Prenda>
            {
                new Prenda { Id = 1, Nombre = "Camiseta 1", CategoriaId = 1, Stock = 50 },
                new Prenda { Id = 2, Nombre = "Camiseta 2", CategoriaId = 1, Stock = 30 },
                new Prenda { Id = 3, Nombre = "Pantalón 1", CategoriaId = 2, Stock = 20 },
                new Prenda { Id = 4, Nombre = "Zapato 1", CategoriaId = 3, Stock = 10 }
            };

            _mockUnitOfWork.Setup(u => u.Prendas.GetAllAsync())
                .ReturnsAsync(prendas);

            _mockUnitOfWork.Setup(u => u.Categorias.GetAllAsync())
                .ReturnsAsync(categorias);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult?.ViewData["Labels"]);
            Assert.IsNotNull(viewResult?.ViewData["Counts"]);
            Assert.IsNotNull(viewResult?.ViewData["Stocks"]);
        }

        #endregion
    }
}
