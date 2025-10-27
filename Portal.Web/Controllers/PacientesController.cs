using System;
using System.Collections.Generic;
using System.Linq;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.Mappers;
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

        public PacientesController(IPacienteAppService pacienteAppService, IUsuarioAppService usuarioAppService)
        {
            _pacienteAppService = pacienteAppService;
            _usuarioAppService = usuarioAppService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .OrderByDescending(p => p.DataCadastro)
                .Select(PacienteViewModelMapper.ToListItem)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
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

            var paciente = model.ToEntity();

            try
            {
                await _pacienteAppService.CreateAsync(paciente);
            }
            catch (Exception ex)
            {
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
            model.RiscoQuedasSelecionados ??= new List<string>();
            model.MobilidadeSelecionada ??= new List<string>();
            model.DietasSelecionadas ??= new List<string>();

            model.CondicoesCronicasDisponiveis = CriarSelectList<Enums.CondicaoCronicaPaciente>(model.CondicoesCronicasSelecionadas);
            model.HistoricoCirurgicoDisponiveis = CriarSelectList<Enums.HistoricoCirurgicoPaciente>(model.HistoricoCirurgicoSelecionados);
            model.RiscoQuedasDisponiveis = CriarSelectList<Enums.RiscoQuedaPaciente>(model.RiscoQuedasSelecionados);
            model.MobilidadeDisponivel = CriarSelectList<Enums.MobilidadePaciente>(model.MobilidadeSelecionada);
            model.DietasDisponiveis = CriarSelectList<Enums.DietaRestricaoPaciente>(model.DietasSelecionadas);
        }

        private static IEnumerable<SelectListItem> CriarSelectList<TEnum>(IEnumerable<string> selecionados)
            where TEnum : struct, Enum
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
    }
}