using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.Mappers;
using GestaoSaudeIdosos.Web.Services;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Web.Extensions;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        private readonly IPacienteAppService _pacienteAppService;
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IImagemStorageService _imagemStorageService;

        public PacientesController(IPacienteAppService pacienteAppService, IUsuarioAppService usuarioAppService, IImagemStorageService imagemStorageService)
        {
            _pacienteAppService = pacienteAppService;
            _usuarioAppService = usuarioAppService;
            _imagemStorageService = imagemStorageService;
        }

        public async Task<IActionResult> Index([FromQuery] PacienteFiltroViewModel filtro)
        {
            filtro ??= new PacienteFiltroViewModel();

            var query = _pacienteAppService.AsQueryable(p => p.Responsavel);

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
                query = query.Where(p => EF.Functions.ILike(p.NomeCompleto, $"%{filtro.Busca.Trim()}%") || EF.Functions.ILike(p.CpfRg ?? string.Empty, $"%{filtro.Busca.Trim()}%"));
            if (filtro.ResponsavelId.HasValue)
                query = query.Where(p => p.ResponsavelId == filtro.ResponsavelId.Value);

            var itensPorPagina = filtro.ItensPorPagina;
            var totalRegistros = await query.CountAsync();
            var totalPaginas = totalRegistros is 0 ? 0 : (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var paginaAtual = filtro.Pagina;
            if (totalPaginas > 0 && paginaAtual > totalPaginas)
                paginaAtual = totalPaginas;

            var registros = await query
                .OrderByDescending(p => p.DataCadastro)
                .Skip((paginaAtual - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .Select(PacienteViewModelMapper.ToListItem)
                .ToListAsync();

            filtro.Pagina = paginaAtual;

            var responsaveis = await ObterResponsaveisAsync();

            var model = new PacientesIndexViewModel
            {
                Filtro = filtro,
                Paginacao = new PaginacaoViewModel
                {
                    PaginaAtual = paginaAtual,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = totalRegistros,
                    ItensPorPagina = itensPorPagina
                },
                Registros = registros,
                Responsaveis = responsaveis
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .Include(p => p.Resultados)
                    .ThenInclude(r => r.Formulario)
                .Include(p => p.Resultados)
                    .ThenInclude(r => r.UsuarioAplicacao)
                .Include(p => p.Resultados)
                    .ThenInclude(r => r.Valores)
                        .ThenInclude(v => v.Campo)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            var detalhe = paciente.ToDetalhe();

            return View(detalhe);
        }

        public async Task<IActionResult> Create()
        {
            var responsaveis = await ObterResponsaveisAsync();
            var model = new PacienteFormViewModel
            {
                Responsaveis = responsaveis
            };

            PreencherOpcoesClinicas(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteFormViewModel model)
        {
            var responsaveis = await ObterResponsaveisAsync();
            model.Responsaveis = responsaveis;
            PreencherOpcoesClinicas(model);

            if (!ModelState.IsValid)
                return View(model);

            if (!model.DataNascimento.HasValue)
            {
                ModelState.AddModelError(nameof(model.DataNascimento), "Informe a data de nascimento.");
                return View(model);
            }

            model.ImagemPerfil = await _imagemStorageService.SalvarAsync(model.ImagemPerfilArquivo, "pacientes");

            var paciente = model.ToEntity();

            try
            {
                await _pacienteAppService.CreateAsync(paciente);
            }
            catch (Exception ex)
            {
                _imagemStorageService.Remover(model.ImagemPerfil);

                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível cadastrar o paciente. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Paciente cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var responsaveis = await ObterResponsaveisAsync();

            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            var model = paciente.ToFormViewModel();
            model.Responsaveis = responsaveis;
            PreencherOpcoesClinicas(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PacienteFormViewModel model)
        {
            var responsaveis = await ObterResponsaveisAsync();
            model.Responsaveis = responsaveis;
            PreencherOpcoesClinicas(model);

            if (model.PacienteId is null || model.PacienteId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o paciente informado.");

            if (!ModelState.IsValid)
                return View(model);

            var paciente = await _pacienteAppService
                .AsTracking(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            if (!model.DataNascimento.HasValue)
            {
                ModelState.AddModelError(nameof(model.DataNascimento), "Informe a data de nascimento.");
                return View(model);
            }

            if (model.ImagemPerfilArquivo is { Length: > 0 })
            {
                model.RemoverImagemPerfil = false;
            }

            if (model.RemoverImagemPerfil)
            {
                _imagemStorageService.Remover(paciente.ImagemPerfil);
                model.ImagemPerfil = null;
            }
            else
            {
                var imagemAtualizada = await _imagemStorageService.SalvarAsync(model.ImagemPerfilArquivo, "pacientes", paciente.ImagemPerfil);
                model.ImagemPerfil = imagemAtualizada;
            }

            model.ApplyToEntity(paciente);

            try
            {
                _pacienteAppService.Update(paciente);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível atualizar os dados do paciente. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Dados do paciente atualizados com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            var model = paciente.ToDetalhe();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var paciente = await _pacienteAppService
                .AsTracking(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            _imagemStorageService.Remover(paciente.ImagemPerfil);

            _pacienteAppService.Delete(paciente);

            TempData["Sucesso"] = "Paciente removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> ObterResponsaveisAsync()
        {
            var usuarios = await _usuarioAppService
                .AsQueryable()
                .OrderBy(u => u.NomeCompleto)
                .ToListAsync();

            return usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.UsuarioId.ToString(),
                    Text = u.NomeCompleto
                })
                .ToList();
        }

        private static void PreencherOpcoesClinicas(PacienteFormViewModel model)
        {
            model.CondicoesCronicasSelecionadas ??= new List<string>();
            model.HistoricoCirurgicoSelecionados ??= new List<string>();
            model.DietasSelecionadas ??= new List<string>();
            model.RiscoQueda ??= Enums.RiscoQuedaPaciente.SemRisco;
            model.Mobilidade ??= Enums.MobilidadePaciente.Independente;
            model.Sexo ??= Enums.SexoPaciente.NaoInformado;

            model.CondicoesCronicasDisponiveis = CriarSelectList<Enums.CondicaoCronicaPaciente>(model.CondicoesCronicasSelecionadas);
            model.HistoricoCirurgicoDisponiveis = CriarSelectList<Enums.HistoricoCirurgicoPaciente>(model.HistoricoCirurgicoSelecionados);
            model.DietasDisponiveis = CriarSelectList<Enums.DietaRestricaoPaciente>(model.DietasSelecionadas);
            model.RiscoQuedasDisponiveis = CriarSelectList(model.RiscoQueda);
            model.MobilidadeDisponivel = CriarSelectList(model.Mobilidade);
            model.SexosDisponiveis = CriarSelectList(model.Sexo);
        }

        private static IEnumerable<SelectListItem> CriarSelectList<TEnum>(IEnumerable<string> selecionados) where TEnum : struct, Enum
        {
            var selecionadosSet = new HashSet<string>(selecionados ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(valor => new SelectListItem
                {
                    Value = valor.ToString(),
                    Text = valor.GetDisplayName(),
                    Selected = selecionadosSet.Contains(valor.ToString())
                })
                .ToList();
        }

        private static IEnumerable<SelectListItem> CriarSelectList<TEnum>(TEnum? selecionado) where TEnum : struct, Enum
        {
            var selecionadoValor = selecionado?.ToString();

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(valor => new SelectListItem
                {
                    Value = valor.ToString(),
                    Text = valor.GetDisplayName(),
                    Selected = selecionadoValor == valor.ToString()
                })
                .ToList();
        }
    }
}