using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var pacientes = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .OrderByDescending(p => p.DataCadastro)
                .ToListAsync();

            var model = pacientes
                .Select(p => new PacienteListItemViewModel
                {
                    PacienteId = p.PacienteId,
                    Nome = p.Nome,
                    DataNascimento = p.DataNascimento,
                    Idade = p.Idade,
                    Responsavel = p.Responsavel?.Nome,
                    DataCadastro = p.DataCadastro
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (paciente is null)
                return NotFound();

            var detalhe = new PacienteDetalheViewModel
            {
                PacienteId = paciente.PacienteId,
                Nome = paciente.Nome,
                Idade = paciente.Idade,
                DataNascimento = paciente.DataNascimento,
                Responsavel = paciente.Responsavel?.Nome ?? string.Empty,
                UltimaAtualizacao = paciente.DataCadastro,
                FormulariosRecentes = Array.Empty<PacienteFormularioResultadoViewModel>(),
                //GraficosPersonalizados = Array.Empty<PacienteGraficoResumoViewModel>()
            };

            return View(detalhe);
        }

        public async Task<IActionResult> Create()
        {
            var responsaveis = await ObterResponsaveisAsync();
            var model = new PacienteFormViewModel
            {
                Responsaveis = responsaveis
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteFormViewModel model)
        {
            var responsaveis = await ObterResponsaveisAsync();
            model.Responsaveis = responsaveis;

            if (!ModelState.IsValid)
                return View(model);

            if (!model.DataNascimento.HasValue)
            {
                ModelState.AddModelError(nameof(model.DataNascimento), "Informe a data de nascimento.");
                return View(model);
            }

            var paciente = new Paciente
            {
                Nome = model.Nome.Trim(),
                DataNascimento = model.DataNascimento.Value,
                Idade = CalcularIdade(model.DataNascimento.Value),
                ResponsavelId = model.ResponsavelId
            };

            await _pacienteAppService.CreateAsync(paciente);

            TempData["Sucesso"] = "Paciente cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var paciente = await _pacienteAppService
                .AsQueryable(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.PacienteId == id);
            if (paciente is null)
                return NotFound();

            var responsaveis = await ObterResponsaveisAsync();

            var model = new PacienteFormViewModel
            {
                PacienteId = paciente.PacienteId,
                Nome = paciente.Nome,
                DataNascimento = paciente.DataNascimento,
                ResponsavelId = paciente.ResponsavelId,
                Responsaveis = responsaveis
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PacienteFormViewModel model)
        {
            var responsaveis = await ObterResponsaveisAsync();
            model.Responsaveis = responsaveis;

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

            paciente.Nome = model.Nome.Trim();
            paciente.DataNascimento = model.DataNascimento.Value;
            paciente.Idade = CalcularIdade(model.DataNascimento.Value);
            paciente.ResponsavelId = model.ResponsavelId;

            _pacienteAppService.Update(paciente);

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

            var model = new PacienteDetalheViewModel
            {
                PacienteId = paciente.PacienteId,
                Nome = paciente.Nome,
                Idade = paciente.Idade,
                DataNascimento = paciente.DataNascimento,
                Responsavel = paciente.Responsavel?.Nome ?? string.Empty,
                UltimaAtualizacao = paciente.DataCadastro,
                FormulariosRecentes = Array.Empty<PacienteFormularioResultadoViewModel>(),
                //GraficosPersonalizados = Array.Empty<PacienteGraficoResumoViewModel>()
            };

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

        private static int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }

        private async Task<IEnumerable<SelectListItem>> ObterResponsaveisAsync()
        {
            var usuarios = await _usuarioAppService
                .AsQueryable()
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.UsuarioId.ToString(),
                    Text = u.Nome
                })
                .ToList();
        }
    }
}
