using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class GraficoAppService : AppService<Grafico>, IGraficoAppService
    {
        public GraficoAppService(IGraficoService service) : base(service)
        {
        }
    }
}
