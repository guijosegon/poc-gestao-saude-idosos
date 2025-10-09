using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class CampoService : Service<Campo>, ICampoService
    {
        public CampoService(ICampoRepository repository) : base(repository)
        {
        }
    }
}
