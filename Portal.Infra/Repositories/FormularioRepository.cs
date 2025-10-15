using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class FormularioRepository : Repository<Formulario>, IFormularioRepository
    {
        public FormularioRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
        }
    }
}
