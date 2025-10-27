using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GestaoSaudeIdosos.Web.Services
{
    public interface IImagemStorageService
    {
        Task<string?> SalvarAsync(IFormFile? arquivo, string subpasta, string? caminhoAtual = null, CancellationToken cancellationToken = default);
        void Remover(string? caminho);
    }
}
