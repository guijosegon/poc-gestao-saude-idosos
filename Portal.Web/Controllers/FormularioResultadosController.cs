using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.Mappers;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class FormularioResultadosController : Controller
    {
        private readonly IFormularioAppService _formularioAppService;
        private readonly IPacienteAppService _pacienteAppService;
        private readonly IFormularioResultadoAppService _formularioResultadoAppService;
        private readonly IUsuarioAppService _usuarioAppService;

        public FormularioResultadosController(
            IFormularioAppService formularioAppService,
            IPacienteAppService pacienteAppService,
            IFormularioResultadoAppService formularioResultadoAppService,
            IUsuarioAppService usuarioAppService)
        {
            _formularioAppService = formularioAppService;
            _pacienteAppService = pacienteAppService;
            _formularioResultadoAppService = formularioResultadoAppService;
            _usuarioAppService = usuarioAppService;
        }

        [HttpGet]
        public async Task<IActionResult> SelecionarPaciente(int formularioId)
        {
            var formulario = await _formularioAppService.AsQueryable().FirstOrDefaultAsync(f => f.FormularioId == formularioId);
            if (formulario is null)
                return NotFound();

            var pacientes = await _pacienteAppService.AsQueryable()
                .Where(p => p.Ativo)
                .OrderBy(p => p.NomeCompleto)
                .Select(p => new FormularioAplicacaoSelecaoItemViewModel
                {
                    Id = p.PacienteId,
                    Titulo = p.NomeCompleto,
                    Descricao = string.IsNullOrWhiteSpace(p.CpfRg) ? null : p.CpfRg,
                    Complemento = p.Responsavel != null ? p.Responsavel.NomeCompleto : null
                })
                .ToListAsync();

            ViewData["SelecaoTipo"] = "paciente";

            var model = new FormularioAplicacaoSelecaoViewModel
            {
                Titulo = "Selecione o paciente",
                Descricao = $"Escolha quem receberá o formulário \"{formulario.Descricao}\".",
                Itens = pacientes
            };

            return PartialView("_SelecaoModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> SelecionarFormulario(int pacienteId)
        {
            var paciente = await _pacienteAppService.AsQueryable().FirstOrDefaultAsync(p => p.PacienteId == pacienteId);
            if (paciente is null)
                return NotFound();

            var formularios = await _formularioAppService.AsQueryable(f => f.Campos)
                .Where(f => f.Ativo)
                .OrderBy(f => f.Descricao)
                .Select(f => new FormularioAplicacaoSelecaoItemViewModel
                {
                    Id = f.FormularioId,
                    Titulo = f.Descricao,
                    Descricao = f.Campos != null ? $"{f.Campos.Count} campo(s)" : null
                })
                .ToListAsync();

            ViewData["SelecaoTipo"] = "formulario";

            var model = new FormularioAplicacaoSelecaoViewModel
            {
                Titulo = "Selecione o formulário",
                Descricao = $"Escolha qual formulário aplicar em {paciente.NomeCompleto}.",
                Itens = formularios
            };

            return PartialView("_SelecaoModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> Aplicar(int formularioId, int pacienteId, string? abaOrigem, string? urlOrigem)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(formularioId);
            if (formulario is null || !formulario.Ativo)
                return NotFound();

            var paciente = await _pacienteAppService.AsQueryable().FirstOrDefaultAsync(p => p.PacienteId == pacienteId);
            if (paciente is null)
                return NotFound();

            if (formulario.Campos is null || !formulario.Campos.Any())
            {
                ViewData["Erro"] = "Este formulário ainda não possui campos configurados.";
                return View("Aplicar", new FormularioAplicacaoViewModel
                {
                    FormularioId = formulario.FormularioId,
                    PacienteId = paciente.PacienteId,
                    FormularioDescricao = formulario.Descricao,
                    PacienteNome = paciente.NomeCompleto,
                    AbaOrigem = abaOrigem,
                    UrlOrigem = urlOrigem
                });
            }

            var model = FormularioResultadoViewModelMapper.CriarAplicacaoViewModel(formulario, paciente, abaOrigem, urlOrigem);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aplicar(FormularioAplicacaoInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ReexibirAplicacaoAsync(model, "Revise os dados informados.");
            }

            var formulario = await _formularioAppService.GetCompletoPorIdAsync(model.FormularioId);
            if (formulario is null || !formulario.Ativo)
                return NotFound();

            var paciente = await _pacienteAppService.AsQueryable().FirstOrDefaultAsync(p => p.PacienteId == model.PacienteId);
            if (paciente is null)
                return NotFound();

            if (formulario.Campos is null || !formulario.Campos.Any())
            {
                return await ReexibirAplicacaoAsync(model, "Este formulário ainda não possui campos configurados.");
            }

            var campos = formulario.Campos.ToDictionary(fc => fc.FormularioCampoId);
            var valores = new List<FormularioResultadoValor>();

            for (var index = 0; index < model.Campos.Count; index++)
            {
                var campoModel = model.Campos[index];

                if (!campos.TryGetValue(campoModel.FormularioCampoId, out var configuracao))
                    continue;

                var tipo = configuracao.Campo?.Tipo ?? Enums.TipoCampo.Texto;
                var normalizado = NormalizarValor(campoModel, tipo, configuracao.Campo);

                if (!normalizado.Valido)
                {
                    ModelState.AddModelError($"Campos[{index}].Valor", $"Informe um valor válido para {configuracao.Campo?.Descricao}.");
                    continue;
                }

                if (configuracao.Obrigatorio && string.IsNullOrWhiteSpace(normalizado.Valor))
                {
                    ModelState.AddModelError($"Campos[{index}].Valor", $"O campo {configuracao.Campo?.Descricao} é obrigatório.");
                    continue;
                }

                valores.Add(new FormularioResultadoValor
                {
                    CampoId = configuracao.CampoId,
                    FormularioCampoId = configuracao.FormularioCampoId,
                    Valor = normalizado.Valor,
                    DataPreenchimento = DateTime.UtcNow
                });
            }

            if (!ModelState.IsValid)
            {
                return await ReexibirAplicacaoAsync(model, "Não foi possível aplicar o formulário. Corrija os erros destacados.");
            }

            var usuarioAplicacaoId = await ObterUsuarioAtualAsync();

            var resultado = new FormularioResultado
            {
                FormularioId = formulario.FormularioId,
                PacienteId = paciente.PacienteId,
                UsuarioAplicacaoId = usuarioAplicacaoId,
                DataPreenchimento = DateTime.UtcNow,
                Valores = valores
            };

            await _formularioResultadoAppService.RegistrarResultadoAsync(resultado);

            TempData["Sucesso"] = "Formulário aplicado com sucesso.";

            var retorno = ObterUrlRetorno(model.UrlOrigem);
            return Redirect(retorno);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string? abaOrigem, string? urlOrigem)
        {
            var resultado = await _formularioResultadoAppService.ObterCompletoAsync(id);
            if (resultado is null)
                return NotFound();

            var model = FormularioResultadoViewModelMapper.CriarDetalheViewModel(resultado, abaOrigem, urlOrigem);
            return View(model);
        }

        private async Task<IActionResult> ReexibirAplicacaoAsync(FormularioAplicacaoInputModel model, string mensagem)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(model.FormularioId);
            var paciente = await _pacienteAppService.AsQueryable().FirstOrDefaultAsync(p => p.PacienteId == model.PacienteId);

            if (formulario is null || paciente is null)
                return NotFound();

            ViewData["Erro"] = mensagem;

            var viewModel = FormularioResultadoViewModelMapper.CriarAplicacaoViewModel(formulario, paciente, model);
            return View("Aplicar", viewModel);
        }

        private string ObterUrlRetorno(string? urlOrigem)
        {
            if (!string.IsNullOrWhiteSpace(urlOrigem) && Url.IsLocalUrl(urlOrigem))
            {
                return urlOrigem;
            }

            return Url.Action("Index", "Formularios") ?? "/";
        }

        private async Task<int?> ObterUsuarioAtualAsync()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();
            return usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))?.UsuarioId;
        }

        private static (string? Valor, bool Valido) NormalizarValor(FormularioAplicacaoCampoInputModel campoModel, Enums.TipoCampo tipo, Campo? campo)
        {
            switch (tipo)
            {
                case Enums.TipoCampo.Numero:
                    if (string.IsNullOrWhiteSpace(campoModel.Valor))
                        return (null, true);

                    if (decimal.TryParse(campoModel.Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var numero))
                        return (numero.ToString(CultureInfo.InvariantCulture), true);

                    return (null, false);
                case Enums.TipoCampo.Data:
                case Enums.TipoCampo.DataHora:
                    if (string.IsNullOrWhiteSpace(campoModel.Valor))
                        return (null, true);

                    if (DateTime.TryParse(campoModel.Valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data))
                        return (tipo == Enums.TipoCampo.Data ? data.ToString("yyyy-MM-dd") : data.ToString("o"), true);

                    if (DateTime.TryParse(campoModel.Valor, out data))
                        return (tipo == Enums.TipoCampo.Data ? data.ToString("yyyy-MM-dd") : data.ToString("o"), true);

                    return (null, false);
                case Enums.TipoCampo.Selecao:
                    if (string.IsNullOrWhiteSpace(campoModel.Valor))
                        return (null, true);

                    if (campo?.Opcoes is not null && campo.Opcoes.Any())
                    {
                        return campo.Opcoes.Contains(campoModel.Valor, StringComparer.OrdinalIgnoreCase)
                            ? (campoModel.Valor, true)
                            : (null, false);
                    }

                    return (campoModel.Valor, true);
                case Enums.TipoCampo.Checkbox:
                    return (campoModel.ValorBooleano ? "true" : "false", true);
                default:
                    return string.IsNullOrWhiteSpace(campoModel.Valor)
                        ? (null, true)
                        : (campoModel.Valor.Trim(), true);
            }
        }
    }
}
