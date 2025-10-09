using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class GraficoService : Service<Grafico>, IGraficoService
    {
        public GraficoService(IGraficoRepository repository) : base(repository)
        {
        }
    }
}
