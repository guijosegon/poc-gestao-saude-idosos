using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;

namespace GestaoSaudeIdosos.Domain.Services
{
    internal class FormularioService : Service<Formulario>, IFormularioService
    {
        public FormularioService(IFormularioRepository repository) : base(repository)
        {
        }
    }
}
