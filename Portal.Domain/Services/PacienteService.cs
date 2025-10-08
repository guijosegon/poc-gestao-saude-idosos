using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class PacienteService : Service<Paciente>, IPacienteService
    {
        public PacienteService(IPacienteRepository repository) : base(repository)
        {
        }
    }
}
