using System.Linq;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes);
        IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes);
        IQueryable<T> Query();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
