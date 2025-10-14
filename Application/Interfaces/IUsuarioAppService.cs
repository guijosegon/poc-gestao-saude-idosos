using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Application.Interfaces
{
    public interface IUsuarioAppService : IAppService<Usuario>
    {
        Usuario? GetByEmail(string email);
        bool VerifyPassword(Usuario usuario, string senha);
    }
}
