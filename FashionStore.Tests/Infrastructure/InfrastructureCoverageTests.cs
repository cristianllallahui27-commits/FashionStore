using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using FashionStore.Infrastructure.Repositories;
using FashionStore.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Tests.InfraCoverage
{
    [TestClass]
    public class InfrastructureCoverageTests
    {
        private FashionStoreDbContext _context = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FashionStoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"FashionStoreDb_{Guid.NewGuid()}")
                .Options;

            _context = new FashionStoreDbContext(options);
        }

        [TestCleanup]
        public void Cleanup() => _context?.Dispose();

        [TestMethod]
        public void UnitOfWork_Exposes_All_GenericRepositories()
        {
            var uow = new FashionStore.Infrastructure.UnitOfWork.UnitOfWork(_context);

            Assert.IsNotNull(uow.Categorias);
            Assert.IsNotNull(uow.Prendas);
            Assert.IsNotNull(uow.Clientes);
            Assert.IsNotNull(uow.Vendedores);
            Assert.IsNotNull(uow.Ventas);
            Assert.IsNotNull(uow.DetalleVentas);
            Assert.IsNotNull(uow.MetodosPago);

            // Types
            Assert.IsInstanceOfType(uow.Clientes, typeof(GenericRepository<Cliente>));
            Assert.IsInstanceOfType(uow.Vendedores, typeof(GenericRepository<Vendedor>));
        }

        [TestMethod]
        public async Task GenericRepository_CanAddAndRetrieve_Cliente()
        {
            var repo = new GenericRepository<Cliente>(_context);

            var cliente = new Cliente { NombreCompleto = "Cliente Test", DNI = "12345678", Telefono = "555-1234", Direccion = "Calle Falsa 123" };
            await repo.AddAsync(cliente);
            await _context.SaveChangesAsync();

            var all = await repo.GetAllAsync();
            Assert.IsTrue(all.Any(c => c.NombreCompleto == "Cliente Test"));
        }

        [TestMethod]
        public async Task GenericRepository_CanAddAndRetrieve_Vendedor()
        {
            var repo = new GenericRepository<Vendedor>(_context);

            var vend = new Vendedor { Nombres = "VendedorTest" };
            await repo.AddAsync(vend);
            await _context.SaveChangesAsync();

            var all = await repo.GetAllAsync();
            Assert.IsTrue(all.Any(v => v.Nombres == "VendedorTest"));
        }

        [TestMethod]
        public void DbContext_OnModelCreating_Configures_Prenda_NameMaxLength_And_Precision()
        {
            var model = _context.Model;

            var prendaEntity = model.FindEntityType(typeof(Prenda));
            Assert.IsNotNull(prendaEntity);

            var nombreProp = prendaEntity.FindProperty("Nombre");
            Assert.IsNotNull(nombreProp);

            var maxLen = nombreProp.GetMaxLength();
            // [StringLength(150)] in Prenda.cs
            Assert.AreEqual(150, maxLen);

            var precioProp = prendaEntity.FindProperty("Precio");
            Assert.IsNotNull(precioProp);

            var precision = precioProp.GetPrecision();
            var scale = precioProp.GetScale();

            Assert.AreEqual(10, precision);
            Assert.AreEqual(2, scale);
        }
    }
}
