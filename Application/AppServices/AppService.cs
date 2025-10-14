using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class AppService<T> : IAppService<T> where T : class
    {
        private readonly IService<T> _service;

        public AppService(IService<T> service) => _service = service;

        public virtual IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes) =>
            _service.AsQueryable(includes);

        public virtual IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes) =>
            _service.AsTracking(includes);

        public virtual async Task CreateAsync(T entity) => await _service.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _service.GetAllAsync();

        public async Task<T?> GetByIdAsync(int id) => await _service.GetByIdAsync(id);

        public virtual void Delete(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _service.Remove(entity);
        }

        public virtual void Update(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _service.Update(entity);
        }
    }
}
