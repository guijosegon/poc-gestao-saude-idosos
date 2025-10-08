using GestaoSaudeIdosos.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoSaudeIdosos.Domain.Interfaces.Repositories
{
    public interface IFormularioCampoRepository : IRepository<FormularioCampo>
    {
        Task<IList<FormularioCampo>> ListarPorFormularioAsync(int formularioId);
    }
}
