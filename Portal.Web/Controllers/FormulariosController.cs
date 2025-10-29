using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.Mappers;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class FormulariosController : Controller
    {
        private readonly IFormularioAppService _formularioAppService;
        private readonly ICampoAppService _campoAppService;
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IFormularioResultadoAppService _formularioResultadoAppService;

        public FormulariosController(
            IFormularioAppService formularioAppService,
            ICampoAppService campoAppService,
            IUsuarioAppService usuarioAppService,
            IFormularioResultadoAppService formularioResultadoAppService)
        {
            _formularioAppService = formularioAppService;
            _campoAppService = campoAppService;
            _usuarioAppService = usuarioAppService;
            _formularioResultadoAppService = formularioResultadoAppService;
        }

        public async Task<IActionResult> Index([FromQuery] FormularioFiltroViewModel filtro)
        {
            filtro ??= new FormularioFiltroViewModel();

            var query = _formularioAppService.AsQueryable(f => f.Campos, f => f.Pacientes);

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
                query = query.Where(f => EF.Functions.ILike(f.Descricao, $"%{filtro.Busca.Trim()}%"));
            if (filtro.Ativo.HasValue)
                query = query.Where(f => f.Ativo == filtro.Ativo.Value);

            var itensPorPagina = filtro.ItensPorPagina;
            var totalRegistros = await query.CountAsync();
            var totalPaginas = totalRegistros is 0 ? 0 : (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var paginaAtual = filtro.Pagina;
            if (totalPaginas > 0 && paginaAtual > totalPaginas) paginaAtual = totalPaginas;

            var registros = await query
                .OrderByDescending(f => f.DataCadastro)
                .Skip((paginaAtual - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .Select(FormularioViewModelMapper.ToListItem)
                .ToListAsync();

            filtro.Pagina = paginaAtual;

            var model = new FormulariosIndexViewModel
            {
                Filtro = filtro,
                Paginacao = new PaginacaoViewModel
                {
                    PaginaAtual = paginaAtual,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = totalRegistros,
                    ItensPorPagina = itensPorPagina
                },
                Registros = registros
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);

            if (formulario is null)
                return NotFound();

            var detalhes = formulario.ToDetailModel();

            if (formulario.Resultados != null && formulario.Resultados.Any(r => r.Valores is null || r.Valores.Count == 0))
            {
                var resultadosCompletos = await _formularioResultadoAppService.ListarPorFormularioAsync(id);
                detalhes.Aplicacoes = FormularioResultadoViewModelMapper.MapearParaResumo(resultadosCompletos).ToList();
            }

            return View(detalhes);
        }

        public async Task<IActionResult> Create()
        {
            var model = await CriarFormularioAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormularioFormViewModel model)
        {
            model.CamposDisponiveis = await ObterCamposAsync();

            if (!ModelState.IsValid)
                return View(model);

            var usuarioId = await ObterUsuarioAtualAsync();
            var formulario = model.ToEntity(usuarioId);

            try
            {
                await _formularioAppService.CreateAsync(formulario);
                await _formularioAppService.AtualizarCamposAsync(formulario, model.CamposSelecionados);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível configurar o formulário. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Formulário configurado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);

            if (formulario is null)
                return NotFound();

            var camposDisponiveis = await ObterCamposAsync();

            var model = formulario.ToFormViewModel(camposDisponiveis);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormularioFormViewModel model)
        {
            model.CamposDisponiveis = await ObterCamposAsync();

            if (model.FormularioId is null || model.FormularioId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o formulário informado.");

            if (!ModelState.IsValid)
                return View(model);

            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);

            if (formulario is null)
                return NotFound();

            model.ApplyToEntity(formulario);

            try
            {
                _formularioAppService.Update(formulario);
                await _formularioAppService.AtualizarCamposAsync(formulario, model.CamposSelecionados);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível atualizar o formulário. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Alterações registradas com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);

            if (formulario is null)
                return NotFound();

            var model = formulario.ToDetailModel();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var formulario = await _formularioAppService.AsTracking().FirstOrDefaultAsync(f => f.FormularioId == id);

            if (formulario is null)
                return NotFound();

            _formularioAppService.Delete(formulario);

            TempData["Sucesso"] = "Formulário removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<FormularioFormViewModel> CriarFormularioAsync() => new FormularioFormViewModel { CamposDisponiveis = await ObterCamposAsync() };

        private async Task<IEnumerable<SelectListItem>> ObterCamposAsync()
        {
            var campos = await _campoAppService.AsQueryable().ToListAsync();

            return campos
                .Where(c => c.Ativo)
                .OrderBy(c => c.Descricao)
                .Select(c => new SelectListItem
                {
                    Value = c.CampoId.ToString(),
                    Text = c.Descricao
                })
                .ToList();
        }

        private async Task<int?> ObterUsuarioAtualAsync()
        {
            var email = User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();
            return usuarios.FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase))?.UsuarioId;
        }
    }
}