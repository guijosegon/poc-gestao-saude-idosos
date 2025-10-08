using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class CampoAppService : AppService<Campo>, ICampoAppService
    {
        public CampoAppService(ICampoService service) : base(service)
        {
        }
    }
}
