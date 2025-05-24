using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Infra.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppContexto _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppContexto dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity).AsTask();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id).AsTask();

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}