using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class PacienteRepository : Repository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }
    }
}
