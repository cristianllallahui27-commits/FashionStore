using FashionStore.Domain.Entities;

namespace FashionStore.Domain.Interfaces
{
    public interface IBuscadorProductos
    {
        Task<List<Prenda>> BuscarPorNombre(string nombre);
        Task<List<Prenda>> BuscarPorCategoria(int categoriaId);
        Task<List<Prenda>> BuscarPorTalla(string talla);
        Task<List<Prenda>> BuscarPorColor(string color);
        Task<List<Prenda>> ObtenerDisponibles();
        Task<Prenda?> ObtenerPorId(int id);
        Task<List<Prenda>> ObtenerProductosAgotandose();
    }
}
