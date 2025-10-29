using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Application.Interfaces
{
    public interface IFormularioAppService : IAppService<Formulario>
    {
        Task<Formulario?> GetCompletoPorIdAsync(int id);
        Task AtualizarCamposAsync(Formulario formulario, IEnumerable<int> camposIds);
    }
}
