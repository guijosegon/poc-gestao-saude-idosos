using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class FormularioResultadoAppService : AppService<FormularioResultado>, IFormularioResultadoAppService
    {
        private readonly IFormularioResultadoRepository _repository;

        public FormularioResultadoAppService(
            IFormularioResultadoService service,
            IFormularioResultadoRepository repository) : base(service)
        {
            _repository = repository;
        }

        public async Task<FormularioResultado?> ObterCompletoAsync(int id) => await _repository.ObterCompletoPorIdAsync(id);

        public async Task<IReadOnlyCollection<FormularioResultado>> ListarPorPacienteAsync(int pacienteId, int? limite = null)
            => await _repository.ListarPorPacienteAsync(pacienteId, limite);

        public async Task<IReadOnlyCollection<FormularioResultado>> ListarPorFormularioAsync(int formularioId, int? limite = null)
            => await _repository.ListarPorFormularioAsync(formularioId, limite);

        public async Task RegistrarResultadoAsync(FormularioResultado resultado)
        {
            if (resultado is null)
                throw new ArgumentNullException(nameof(resultado));

            await _repository.AddAsync(resultado);
        }
    }
}
