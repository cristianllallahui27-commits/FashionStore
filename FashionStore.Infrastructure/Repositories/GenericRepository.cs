using FashionStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FashionStore.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly FashionStore.Infrastructure.Context.FashionStoreDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(FashionStore.Infrastructure.Context.FashionStoreDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            // Try to find by primary key (int)
            var entity = await _dbSet.FindAsync(id);
            return entity is null ? null : entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
