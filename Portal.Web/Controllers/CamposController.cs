using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Web.Mappers;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class CamposController : Controller
    {
        private readonly ICampoAppService _campoAppService;
        private readonly IFormularioAppService _formularioAppService;

        public CamposController(ICampoAppService campoAppService, IFormularioAppService formularioAppService)
        {
            _campoAppService = campoAppService;
            _formularioAppService = formularioAppService;
        }

        public async Task<IActionResult> Index([FromQuery] CampoFiltroViewModel filtro)
        {
            filtro ??= new CampoFiltroViewModel();

            var query = _campoAppService.AsQueryable(a => a.Usuario, a => a.ResultadoValores);

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim();
                query = query.Where(c => EF.Functions.ILike(c.Descricao, $"%{busca}%"));
            }

            if (filtro.Tipo.HasValue)
            {
                var tipo = filtro.Tipo.Value;
                query = query.Where(c => c.Tipo == tipo);
            }

            if (filtro.Ativo.HasValue)
            {
                var ativo = filtro.Ativo.Value;
                query = query.Where(c => c.Ativo == ativo);
            }

            var itensPorPagina = filtro.ItensPorPagina;
            var totalRegistros = await query.CountAsync();
            var totalPaginas = totalRegistros == 0
                ? 0
                : (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var paginaAtual = filtro.Pagina;
            if (totalPaginas > 0 && paginaAtual > totalPaginas)
                paginaAtual = totalPaginas;

            var registros = await query
                .OrderByDescending(c => c.DataCadastro)
                .Skip((paginaAtual - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .Select(CampoViewModelMapper.ToListItem)
                .ToListAsync();

            foreach (var campo in registros)
            {
                var tipo = Enum.Parse<Enums.TipoCampo>(campo.Tipo, true);
                campo.Tipo = CampoViewModelMapper.ObterDescricaoTipo(tipo);
            }

            filtro.Pagina = paginaAtual;

            var model = new CamposIndexViewModel
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
                TiposCampo = CampoViewModelMapper.ObterTiposCampo()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var campo = await _campoAppService
                .AsQueryable(a => a.Usuario, a => a.ResultadoValores)
                .FirstOrDefaultAsync(f => f.CampoId == id);

            if (campo is null)
                return NotFound();

            var formularios = await _formularioAppService
                .AsQueryable(a => a.Campos, a => a.Pacientes)
                .ToListAsync();

            var detalhes = campo.ToDetail(formularios);

            return View(detalhes);
        }

        public IActionResult Create()
        {
            var model = new CampoFormViewModel
            {
                TiposCampo = CampoViewModelMapper.ObterTiposCampo()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CampoFormViewModel model)
        {
            model.TiposCampo = CampoViewModelMapper.ObterTiposCampo();

            if (!ModelState.IsValid)
                return View(model);

            var campo = model.ToEntity();

            try
            {
                await _campoAppService.CreateAsync(campo);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível cadastrar o campo. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Campo cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var campo = await _campoAppService
                .AsQueryable(a => a.Usuario)
                .FirstOrDefaultAsync(f => f.CampoId == id);

            if (campo is null)
                return NotFound();

            var model = campo.ToFormViewModel(CampoViewModelMapper.ObterTiposCampo());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CampoFormViewModel model)
        {
            model.TiposCampo = CampoViewModelMapper.ObterTiposCampo();

            if (model.CampoId is null || model.CampoId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o campo informado.");

            if (!ModelState.IsValid)
                return View(model);

            var campo = await _campoAppService
                .AsTracking(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.CampoId == id);

            if (campo is null)
                return NotFound();

            model.ApplyToEntity(campo);

            try
            {
                _campoAppService.Update(campo);
            }
            catch (Exception ex)
            {
                ViewData["Erro"] = string.IsNullOrWhiteSpace(ex.Message) ? "Não foi possível atualizar o campo. Tente novamente." : ex.Message;
                return View(model);
            }

            TempData["Sucesso"] = "Campo atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var campo = await _campoAppService
                .AsQueryable(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.CampoId == id);

            if (campo is null)
                return NotFound();

            var formularios = await _formularioAppService
                .AsQueryable(f => f.Campos, f => f.Pacientes)
                .ToListAsync();

            var model = campo.ToDetail(formularios);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var campo = await _campoAppService
                .AsTracking(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.CampoId == id);

            if (campo is null)
                return NotFound();

            _campoAppService.Delete(campo);

            TempData["Sucesso"] = "Campo removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

    }
}
