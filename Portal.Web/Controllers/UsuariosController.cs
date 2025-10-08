using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
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
    public class UsuariosController : Controller
    {
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuariosController(IUsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioAppService.GetAllAsync();
            var model = usuarios
                .OrderByDescending(u => u.DataCadastro)
                .Select(u => new UsuarioListItemViewModel
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Ativo = u.Ativo,
                    DataCadastro = u.DataCadastro
                })
                .ToList();

            return View(model);
        }

        [Authorize(Roles = nameof(Enums.PerfilUsuario.Administrador))]
        public IActionResult Create()
        {
            ViewBag.Perfis = ObterPerfis();
            return View(new UsuarioFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = nameof(Enums.PerfilUsuario.Administrador))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioFormViewModel model)
        {
            ViewBag.Perfis = ObterPerfis();

            if (!ModelState.IsValid)
                return View(model);

            var existente = _usuarioAppService.GetByEmail(model.Email);
            if (existente is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                return View(model);
            }

            var usuario = new Usuario
            {
                Nome = model.Nome.Trim(),
                Email = model.Email.Trim(),
                Senha = model.Senha,
                Perfil = model.Perfil,
                Ativo = model.Ativo
            };

            await _usuarioAppService.CreateAsync(usuario);

            TempData["Sucesso"] = "Usuário criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private static IEnumerable<SelectListItem> ObterPerfis()
        {
            return Enum.GetValues(typeof(Enums.PerfilUsuario))
                .Cast<Enums.PerfilUsuario>()
                .Select(perfil => new SelectListItem
                {
                    Text = perfil.ToString(),
                    Value = perfil.ToString()
                });
        }
    }
}
