using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;

namespace FashionStore.Web.Services
{
    public interface IBuscadorProductosWeb
    {
        Task<List<Prenda>> BuscarPorNombre(string nombre);
        Task<List<Prenda>> BuscarPorCategoria(int categoriaId);
        Task<List<Prenda>> ObtenerTodos();
    }

    public class BuscadorProductos : IBuscadorProductosWeb
    {
        private readonly IUnitOfWork _unitOfWork;

        public BuscadorProductos(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Prenda>> BuscarPorNombre(string nombre)
        {
            return await Task.FromResult(new List<Prenda>());
        }

        public async Task<List<Prenda>> BuscarPorCategoria(int categoriaId)
        {
            return await Task.FromResult(new List<Prenda>());
        }

        public async Task<List<Prenda>> ObtenerTodos()
        {
            return await Task.FromResult(new List<Prenda>());
        }
    }
}
