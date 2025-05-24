using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public Service(IRepository<T> repository) => _repository = repository;

        public async Task AddAsync(T entity) => await _repository.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<T?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public void Remove(T entity) => _repository.Remove(entity);

        public void Update(T entity) => _repository.Update(entity);
    }
}