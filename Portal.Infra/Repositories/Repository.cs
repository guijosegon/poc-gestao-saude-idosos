using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Infra.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly Contexts.AppContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(Contexts.AppContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id).AsTask();

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
            _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
