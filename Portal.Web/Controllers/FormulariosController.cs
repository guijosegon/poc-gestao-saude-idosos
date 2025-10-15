using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class FormulariosController : Controller
    {
        private readonly IFormularioAppService _formularioAppService;
        private readonly ICampoAppService _campoAppService;
        private readonly IUsuarioAppService _usuarioAppService;

        public FormulariosController(
            IFormularioAppService formularioAppService,
            ICampoAppService campoAppService,
            IUsuarioAppService usuarioAppService)
        {
            _formularioAppService = formularioAppService;
            _campoAppService = campoAppService;
            _usuarioAppService = usuarioAppService;
        }

        public async Task<IActionResult> Index()
        {
            var formularios = await _formularioAppService.AsQueryable().ToListAsync();

            var model = formularios
                .Select(f => new FormularioListItemViewModel
                {
                    FormularioId = f.FormularioId,
                    Descricao = f.Descricao,
                    QuantidadeCampos = f.Campos?.Count ?? 0,
                    QuantidadePacientes = f.Pacientes?.Count ?? 0,
                    Ativo = f.Ativo,
                    DataCadastro = f.DataCadastro
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);
            if (formulario is null)
                return NotFound();

            var detalhes = new FormularioDetalheViewModel
            {
                FormularioId = formulario.FormularioId,
                Descricao = formulario.Descricao,
                Ativo = formulario.Ativo,
                Responsavel = formulario.Usuario?.Nome,
                DataCadastro = formulario.DataCadastro,
                Campos = formulario.Campos?
                    .OrderBy(c => c.Ordem)
                    .Select(c => new FormularioCampoResumoViewModel
                    {
                        NomeCampo = c.Campo?.Descricao ?? string.Empty,
                        Tipo = c.Campo is null ? string.Empty : c.Campo.Tipo.ToString(),
                        Obrigatorio = c.Obrigatorio,
                        Ordem = c.Ordem
                    })
                    .ToList() ?? Enumerable.Empty<FormularioCampoResumoViewModel>()
            };

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

            var formulario = new Formulario
            {
                Descricao = model.Descricao.Trim(),
                Ativo = model.Ativo,
                UsuarioId = await ObterUsuarioAtualAsync()
            };

            await _formularioAppService.CreateAsync(formulario);
            await _formularioAppService.AtualizarCamposAsync(formulario, model.CamposSelecionados);

            TempData["Sucesso"] = "Formulário configurado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);
            if (formulario is null)
                return NotFound();

            var model = new FormularioFormViewModel
            {
                FormularioId = formulario.FormularioId,
                Descricao = formulario.Descricao,
                Ativo = formulario.Ativo,
                CamposSelecionados = formulario.Campos?.Select(c => c.CampoId).ToList() ?? new List<int>(),
                CamposDisponiveis = await ObterCamposAsync()
            };

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

            formulario.Descricao = model.Descricao.Trim();
            formulario.Ativo = model.Ativo;

            _formularioAppService.Update(formulario);
            await _formularioAppService.AtualizarCamposAsync(formulario, model.CamposSelecionados);

            TempData["Sucesso"] = "Alterações registradas com sucesso.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var formulario = await _formularioAppService.GetCompletoPorIdAsync(id);
            if (formulario is null)
                return NotFound();

            var model = new FormularioDetalheViewModel
            {
                FormularioId = formulario.FormularioId,
                Descricao = formulario.Descricao,
                Ativo = formulario.Ativo,
                Responsavel = formulario.Usuario?.Nome,
                DataCadastro = formulario.DataCadastro,
                Campos = formulario.Campos?
                    .OrderBy(c => c.Ordem)
                    .Select(c => new FormularioCampoResumoViewModel
                    {
                        NomeCampo = c.Campo?.Descricao ?? string.Empty,
                        Tipo = c.Campo is null ? string.Empty : c.Campo.Tipo.ToString(),
                        Obrigatorio = c.Obrigatorio,
                        Ordem = c.Ordem
                    })
                    .ToList() ?? Enumerable.Empty<FormularioCampoResumoViewModel>()
            };

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

        private async Task<FormularioFormViewModel> CriarFormularioAsync()
        {
            return new FormularioFormViewModel
            {
                CamposDisponiveis = await ObterCamposAsync()
            };
        }

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
