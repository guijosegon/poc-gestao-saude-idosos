using System.Linq;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Application.Interfaces
{
    public interface IAppService<T> where T : class
    {
        IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes);
        IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
