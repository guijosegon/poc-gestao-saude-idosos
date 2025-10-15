using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class CampoRepository : Repository<Campo>, ICampoRepository
    {
        public CampoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }
    }
}
