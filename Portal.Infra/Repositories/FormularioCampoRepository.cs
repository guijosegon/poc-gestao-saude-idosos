using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class FormularioCampoRepository : Repository<FormularioCampo>, IFormularioCampoRepository
    {
        public FormularioCampoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<FormularioCampo>> ListarPorFormularioAsync(int formularioId)
        {
            return await _dbContext.FormularioCampos
                .Where(fc => fc.FormularioId == formularioId)
                .Include(fc => fc.Campo)
                .OrderBy(fc => fc.Ordem)
                .ToListAsync();
        }
    }
}
