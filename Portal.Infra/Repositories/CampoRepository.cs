using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class CampoRepository : Repository<Campo>, ICampoRepository
    {
        public CampoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Campo>> GetAllAsync()
        {
            return await _dbContext.Set<Campo>()
                .Include(c => c.Usuario)
                .AsNoTracking()
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }

        public override async Task<Campo?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<Campo>()
                .Include(c => c.Usuario)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CampoId == id);
        }
    }
}
