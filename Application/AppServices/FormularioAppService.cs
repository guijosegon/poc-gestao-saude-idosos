using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Interfaces.Repositories;
using GestaoSaudeIdosos.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Application.AppServices
{
    public class FormularioAppService : AppService<Formulario>, IFormularioAppService
    {
        private readonly IFormularioRepository _formularioRepository;
        private readonly IFormularioCampoRepository _formularioCampoRepository;

        public FormularioAppService(
            IFormularioService service,
            IFormularioRepository formularioRepository,
            IFormularioCampoRepository formularioCampoRepository) : base(service)
        {
            _formularioRepository = formularioRepository;
            _formularioCampoRepository = formularioCampoRepository;
        }

        public async Task<Formulario?> GetCompletoPorIdAsync(int id)
        {
            return await _formularioRepository
                .AsQueryable(f => f.Usuario)
                .Include(f => f.Campos)
                    .ThenInclude(fc => fc.Campo)
                .Include(f => f.Pacientes)
                .Include(f => f.Resultados)
                    .ThenInclude(r => r.Paciente)
                .Include(f => f.Resultados)
                    .ThenInclude(r => r.UsuarioAplicacao)
                .Include(f => f.Resultados)
                    .ThenInclude(r => r.Valores)
                        .ThenInclude(v => v.Campo)
                .FirstOrDefaultAsync(f => f.FormularioId == id);
        }

        public async Task AtualizarCamposAsync(Formulario formulario, IEnumerable<int> camposIds)
        {
            if (formulario is null)
                throw new ArgumentNullException(nameof(formulario));

            if (formulario.FormularioId <= 0)
                throw new InvalidOperationException("O formulário precisa estar persistido antes de atualizar os campos.");

            var selecionados = camposIds?.Distinct().ToList() ?? new List<int>();

            var existentes = await _formularioCampoRepository.ListarPorFormularioAsync(formulario.FormularioId);

            foreach (var remover in existentes.Where(fc => !selecionados.Contains(fc.CampoId)).ToList())
            {
                _formularioCampoRepository.Remove(remover);
            }

            var ordem = 1;
            foreach (var campoId in selecionados)
            {
                var existente = existentes.FirstOrDefault(fc => fc.CampoId == campoId);

                if (existente is null)
                {
                    await _formularioCampoRepository.AddAsync(new FormularioCampo
                    {
                        FormularioId = formulario.FormularioId,
                        CampoId = campoId,
                        Ordem = ordem++,
                        Obrigatorio = true
                    });
                }
                else
                {
                    existente.Ordem = ordem++;
                    _formularioCampoRepository.Update(existente);
                }
            }
        }
    }
}
