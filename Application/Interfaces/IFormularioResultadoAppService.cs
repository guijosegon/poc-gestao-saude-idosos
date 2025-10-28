using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Application.Interfaces
{
    public interface IFormularioResultadoAppService : IAppService<FormularioResultado>
    {
        Task<FormularioResultado?> ObterCompletoAsync(int id);
        Task<IReadOnlyCollection<FormularioResultado>> ListarPorPacienteAsync(int pacienteId, int? limite = null);
        Task<IReadOnlyCollection<FormularioResultado>> ListarPorFormularioAsync(int formularioId, int? limite = null);
        Task RegistrarResultadoAsync(FormularioResultado resultado);
    }
}
