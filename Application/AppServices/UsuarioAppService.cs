using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class UsuarioAppService : AppService<Usuario>, IUsuarioAppService
    {
        private readonly IUsuarioService _service;

        public UsuarioAppService(IUsuarioService service) : base(service)
        {
            _service = service;
        }

        public Usuario? GetByEmail(string email) => _service.GetByEmail(email);
    }
}