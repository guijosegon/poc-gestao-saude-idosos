using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Infra.Contexts;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly AppContexto _dbContext;

        public UsuarioRepository(AppContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Usuario? GetByEmail(string email) => _dbContext.Set<Usuario>().FirstOrDefault(u => u.Email == email);
    }
}
