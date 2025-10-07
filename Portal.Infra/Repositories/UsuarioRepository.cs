using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;

namespace GestaoSaudeIdosos.Infra.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly Contexts.AppContext _dbContext;

        public UsuarioRepository(Contexts.AppContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Usuario? GetByEmail(string email) => _dbContext.Set<Usuario>().FirstOrDefault(u => u.Email == email);
    }
}