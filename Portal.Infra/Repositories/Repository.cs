using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public virtual IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    if (include is not null)
                        query = query.Include(include);
                }
            }

            return query;
        }

        public virtual IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    if (include is not null)
                        query = query.Include(include);
                }
            }

            return query.AsTracking();
        }

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
