using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Infrastructure.Services
{
    public class BuscadorProductos : IBuscadorProductos
    {
        private readonly FashionStoreDbContext _context;

        public BuscadorProductos(FashionStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prenda>> BuscarPorNombre(string nombre)
        {
            return await _context.Prendas
                .Where(p => p.Nombre.Contains(nombre))
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<List<Prenda>> BuscarPorCategoria(int categoriaId)
        {
            return await _context.Prendas
                .Where(p => p.CategoriaId == categoriaId)
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<List<Prenda>> BuscarPorTalla(string talla)
        {
            return await _context.Prendas
                .Where(p => p.Talla == talla && p.Stock > 0)
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<List<Prenda>> BuscarPorColor(string color)
        {
            return await _context.Prendas
                .Where(p => p.Color == color && p.Stock > 0)
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<List<Prenda>> ObtenerDisponibles()
        {
            return await _context.Prendas
                .Where(p => p.Stock > 0)
                .Include(p => p.Categoria)
                .OrderByDescending(p => p.Stock)
                .ToListAsync();
        }

        public async Task<Prenda?> ObtenerPorId(int id)
        {
            return await _context.Prendas
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Prenda>> ObtenerProductosAgotandose()
        {
            return await _context.Prendas
                .Where(p => p.Stock > 0 && p.Stock <= 5)
                .Include(p => p.Categoria)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }
    }
}
