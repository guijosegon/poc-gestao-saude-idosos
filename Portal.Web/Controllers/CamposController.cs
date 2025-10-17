using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        public async Task<IActionResult> Index()
        {
            var campos = await _campoAppService
                .AsQueryable(a => a.Usuario)
                .ToListAsync();

            var formularios = await _formularioAppService
                .AsQueryable(a => a.Campos, a => a.Pacientes)
                .ToListAsync();

            var model = campos
                .Select(campo => new CampoListItemViewModel
                {
                    CampoId = campo.CampoId,
                    Descricao = campo.Descricao,
                    Tipo = ObterDescricaoTipo(campo.Tipo),
                    Responsavel = campo.Usuario?.Nome,
                    DataCadastro = campo.DataCadastro,
                    Ativo = campo.Ativo,
                    FormulariosVinculados = formularios.Count(f => f.Campos.Any(fc => fc.CampoId == campo.CampoId))
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var campo = await _campoAppService
                .AsQueryable(a => a.Usuario)
                .FirstOrDefaultAsync(f => f.CampoId == id);

            if (campo is null)
                return NotFound();

            var formularios = await _formularioAppService
                .AsQueryable(a => a.Campos, a => a.Pacientes)
                .ToListAsync();

            var detalhes = new CampoDetalheViewModel
            {
                CampoId = campo.CampoId,
                Descricao = campo.Descricao,
                Tipo = ObterDescricaoTipo(campo.Tipo),
                TextoAjuda = campo.TextoAjuda,
                Opcoes = campo.Opcoes,
                Ativo = campo.Ativo,
                CriadoPor = campo.Usuario?.Nome,
                DataCadastro = campo.DataCadastro,
                FormulariosUtilizacao = formularios.Where(w => w.Campos.Any(a => a.CampoId == campo.CampoId)).Select(s => s.Descricao).ToList()
            };

            return View(detalhes);
        }

        public IActionResult Create()
        {
            var model = CriarFormularioCampo();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CampoFormViewModel model)
        {
            model.TiposCampo = ObterTiposCampo();

            if (!ModelState.IsValid)
                return View(model);

            var campo = new Campo
            {
                Descricao = model.Descricao.Trim(),
                Tipo = Enum.Parse<Enums.TipoCampo>(model.Tipo),
                TextoAjuda = model.TextoAjuda?.Trim(),
                Ativo = model.Ativo,
                Opcoes = ConverterOpcoes(model.Opcoes)
            };

            await _campoAppService.CreateAsync(campo);

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

            var model = new CampoFormViewModel
            {
                CampoId = campo.CampoId,
                Descricao = campo.Descricao,
                Tipo = campo.Tipo.ToString(),
                TextoAjuda = campo.TextoAjuda,
                Opcoes = campo.Opcoes is null ? null : string.Join(Environment.NewLine, campo.Opcoes),
                Ativo = campo.Ativo,
                TiposCampo = ObterTiposCampo()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CampoFormViewModel model)
        {
            model.TiposCampo = ObterTiposCampo();

            if (model.CampoId is null || model.CampoId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o campo informado.");

            if (!ModelState.IsValid)
                return View(model);

            var campo = await _campoAppService
                .AsTracking(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.CampoId == id);

            if (campo is null)
                return NotFound();

            campo.Descricao = model.Descricao.Trim();
            campo.Tipo = Enum.Parse<Enums.TipoCampo>(model.Tipo);
            campo.TextoAjuda = model.TextoAjuda?.Trim();
            campo.Ativo = model.Ativo;
            campo.Opcoes = ConverterOpcoes(model.Opcoes);

            _campoAppService.Update(campo);

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

            var model = new CampoDetalheViewModel
            {
                CampoId = campo.CampoId,
                Descricao = campo.Descricao,
                Tipo = ObterDescricaoTipo(campo.Tipo),
                TextoAjuda = campo.TextoAjuda,
                Opcoes = campo.Opcoes,
                Ativo = campo.Ativo,
                CriadoPor = campo.Usuario?.Nome,
                DataCadastro = campo.DataCadastro,
                FormulariosUtilizacao = formularios
                    .Where(f => f.Campos.Any(fc => fc.CampoId == campo.CampoId))
                    .Select(f => f.Descricao)
                    .ToList()
            };

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

        private static CampoFormViewModel CriarFormularioCampo() => new CampoFormViewModel
        {
            TiposCampo = ObterTiposCampo()
        };

        private static IEnumerable<SelectListItem> ObterTiposCampo()
        {
            return Enum.GetValues(typeof(Enums.TipoCampo))
                .Cast<Enums.TipoCampo>()
                .Select(tipo => new SelectListItem
                {
                    Value = tipo.ToString(),
                    Text = ObterDescricaoTipo(tipo)
                })
                .ToList();
        }

        private static string ObterDescricaoTipo(Enums.TipoCampo tipo)
        {
            var member = typeof(Enums.TipoCampo).GetMember(tipo.ToString()).FirstOrDefault();
            var display = member?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? tipo.ToString();
        }

        private static List<string> ConverterOpcoes(string? opcoes)
        {
            if (string.IsNullOrWhiteSpace(opcoes))
                return new List<string>();

            return opcoes
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrEmpty(o))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
