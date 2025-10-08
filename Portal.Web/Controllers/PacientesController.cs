using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

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
            var pacientes = await _pacienteAppService.GetAllAsync();
            var model = pacientes
                .OrderByDescending(p => p.DataCadastro)
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

        public async Task<IActionResult> Create()
        {
            var model = new PacienteFormViewModel
            {
                Responsaveis = await ObterResponsaveisAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteFormViewModel model)
        {
            model.Responsaveis = await ObterResponsaveisAsync();

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
            var usuarios = await _usuarioAppService.GetAllAsync();

            return usuarios
                .OrderBy(u => u.Nome)
                .Select(u => new SelectListItem
                {
                    Value = u.UsuarioId.ToString(),
                    Text = u.Nome
                })
                .ToList();
        }
    }
}
