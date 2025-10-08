using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class FormularioAppService : AppService<Formulario>, IFormularioAppService
    {
        public FormularioAppService(IFormularioService service) : base(service)
        {
        }
    }
}
