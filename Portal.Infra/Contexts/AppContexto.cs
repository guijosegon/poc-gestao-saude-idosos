using GestaoSaudeIdosos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Infra.Contexts
{
    public class AppContexto : DbContext
    {
        public AppContexto(DbContextOptions<AppContexto> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}