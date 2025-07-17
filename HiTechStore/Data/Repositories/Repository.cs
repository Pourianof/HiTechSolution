using HiTechStore.Core;
using HiTechStore.Core.Repositories;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IModel
    {
        private readonly HiTechStoreDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(HiTechStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<bool> IsExistsAsync(int id)
        {
            var modelType = typeof(T);
            var modelIdName = modelType.GetProperties().FirstOrDefault(p => p.Name.Contains("Id"))?.Name;
            if (modelIdName is null)
            {
                throw new InvalidOperationException("Entity does not have an Id property.");
            }
            return _dbSet.AnyAsync(e => EF.Property<int>(e, modelIdName) == id);
        }
    }
}
