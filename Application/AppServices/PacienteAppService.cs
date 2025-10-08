using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class PacienteAppService : AppService<Paciente>, IPacienteAppService
    {
        public PacienteAppService(IPacienteService service) : base(service)
        {
        }
    }
}
