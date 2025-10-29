using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Domain.Interfaces.Services
{
    public interface IService<T> where T : class
    {
        IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes);
        IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
