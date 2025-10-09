using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class GraficoRepository : Repository<Grafico>, IGraficoRepository
    {
        public GraficoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Grafico>> GetAllAsync()
        {
            return await _dbContext.Graficos
                .AsNoTracking()
                .OrderBy(g => g.Descricao)
                .ToListAsync();
        }

        public override async Task<Grafico?> GetByIdAsync(int id)
        {
            return await _dbContext.Graficos
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GraficoId == id);
        }
    }
}
