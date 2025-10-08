using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class PacienteRepository : Repository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            return await _dbContext.Pacientes
                .Include(p => p.Responsavel)
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public override async Task<Paciente?> GetByIdAsync(int id)
        {
            return await _dbContext.Pacientes
                .Include(p => p.Responsavel)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PacienteId == id);
        }
    }
}
