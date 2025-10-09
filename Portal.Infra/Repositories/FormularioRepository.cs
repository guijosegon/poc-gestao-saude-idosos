using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class FormularioRepository : Repository<Formulario>, IFormularioRepository
    {
        public FormularioRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Formulario>> GetAllAsync()
        {
            return await _dbContext.Formularios
                .Include(f => f.Campos)
                    .ThenInclude(fc => fc.Campo)
                .Include(f => f.Pacientes)
                .AsNoTracking()
                .OrderByDescending(f => f.DataCadastro)
                .ToListAsync();
        }

        public override async Task<Formulario?> GetByIdAsync(int id)
        {
            return await _dbContext.Formularios
                .Include(f => f.Campos)
                    .ThenInclude(fc => fc.Campo)
                .Include(f => f.Pacientes)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FormularioId == id);
        }
    }
}
