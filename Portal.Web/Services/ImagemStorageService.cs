using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GestaoSaudeIdosos.Web.Services
{
    public class ImagemStorageService : IImagemStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImagemStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> SalvarAsync(IFormFile? arquivo, string subpasta, string? caminhoAtual = null, CancellationToken cancellationToken = default)
        {
            if (arquivo is null || arquivo.Length == 0)
            {
                return caminhoAtual;
            }

            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", subpasta);
            Directory.CreateDirectory(uploadsRoot);

            var extensao = Path.GetExtension(arquivo.FileName);
            if (string.IsNullOrWhiteSpace(extensao))
            {
                extensao = ".jpg";
            }

            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            var caminhoFisico = Path.Combine(uploadsRoot, nomeArquivo);

            using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream, cancellationToken);
            }

            RemoverArquivoFisico(caminhoAtual);

            return Path.Combine("~/uploads", subpasta, nomeArquivo).Replace('\\', '/');
        }

        public void Remover(string? caminho)
        {
            RemoverArquivoFisico(caminho);
        }

        private void RemoverArquivoFisico(string? caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                return;
            }

            var relativo = caminho.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
            var caminhoFisico = Path.Combine(_environment.WebRootPath, relativo);

            if (File.Exists(caminhoFisico))
            {
                File.Delete(caminhoFisico);
            }
        }
    }
}
