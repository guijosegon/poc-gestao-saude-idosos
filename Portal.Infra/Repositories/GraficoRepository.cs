using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class GraficoRepository : Repository<Grafico>, IGraficoRepository
    {
        public GraficoRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }
    }
}
