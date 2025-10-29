using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public Service(IRepository<T> repository) => _repository = repository;
        public IQueryable<T> AsQueryable(params Expression<Func<T, object?>>[] includes) => _repository.AsQueryable(includes);
        public IQueryable<T> AsTracking(params Expression<Func<T, object?>>[] includes) => _repository.AsTracking(includes);
        public async Task AddAsync(T entity) => await _repository.AddAsync(entity);
        public void Remove(T entity) => _repository.Remove(entity);
        public void Update(T entity) => _repository.Update(entity);
    }
}
