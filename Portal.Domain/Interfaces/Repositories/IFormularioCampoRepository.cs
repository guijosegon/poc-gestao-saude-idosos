using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Domain.Interfaces.Repositories
{
    public interface IFormularioCampoRepository : IRepository<FormularioCampo>
    {
        Task<IList<FormularioCampo>> ListarPorFormularioAsync(int formularioId);
    }
}
