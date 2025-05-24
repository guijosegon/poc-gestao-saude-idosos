using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class AppService<T> : IAppService<T> where T : class
    {
        private readonly IService<T> _service;

        public AppService(IService<T> service) => _service = service;

        public async Task CreateAsync(T entity) => await _service.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _service.GetAllAsync();

        public async Task<T?> GetByIdAsync(int id) => await _service.GetByIdAsync(id);

        public void Delete(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _service.Remove(entity);
        }

        public void Update(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _service.Update(entity);
        }
    }
}