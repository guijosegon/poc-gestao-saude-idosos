using GestaoSaudeIdosos.Domain.Entities;

namespace GestaoSaudeIdosos.Domain.Interfaces.Repositories
{
    public interface IFormularioResultadoRepository : IRepository<FormularioResultado>
    {
        Task<FormularioResultado?> ObterCompletoPorIdAsync(int id);
        Task<List<FormularioResultado>> ListarPorPacienteAsync(int pacienteId, int? limite = null);
        Task<List<FormularioResultado>> ListarPorFormularioAsync(int formularioId, int? limite = null);
    }
}
