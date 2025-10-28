using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    public class FormularioResultadoService : Service<FormularioResultado>, IFormularioResultadoService
    {
        public FormularioResultadoService(IFormularioResultadoRepository repository) : base(repository)
        {
        }
    }
}
