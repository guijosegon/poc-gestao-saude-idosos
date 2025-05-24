using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Domain.Interfaces.Services
{
    public interface IUsuarioService : IService<Usuario>
    {
        Usuario? GetByEmail(string email);
    }
}