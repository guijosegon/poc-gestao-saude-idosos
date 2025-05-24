using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario? GetByEmail(string email);
    }
}