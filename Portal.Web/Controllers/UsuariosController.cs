using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Common.Helpers;
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
    public class UsuariosController : Controller
    {
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuariosController(IUsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioAppService.AsQueryable().ToListAsync();

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

        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _usuarioAppService.AsQueryable().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            var detalhe = new UsuarioDetalheViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Ativo = usuario.Ativo,
                CriadoEm = usuario.DataCadastro,
                AcoesRecentes = Array.Empty<string>()
            };

            return View(detalhe);
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

            var existente = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email.Equals(model.Email ?? string.Empty));

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

        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _usuarioAppService.AsQueryable().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            var model = new UsuarioEdicaoViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Ativo = usuario.Ativo,
                PerfisDisponiveis = ObterPerfis()
            };

            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, model.Email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            model.PermiteAlterarPerfil = isAdmin;

            if (!isAdmin)
            {
                model.PerfisDisponiveis = model.PerfisDisponiveis
                    .Where(p => string.Equals(p.Value, model.Perfil, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioEdicaoViewModel model)
        {
            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, model.Email, StringComparison.OrdinalIgnoreCase);

            model.PerfisDisponiveis = ObterPerfis();
            model.PermiteAlterarPerfil = isAdmin;

            if (!isAdmin)
            {
                model.PerfisDisponiveis = model.PerfisDisponiveis
                    .Where(p => string.Equals(p.Value, model.Perfil, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (model.UsuarioId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o usuário informado.");

            if (!isAdmin && !isSelf)
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            if (!string.Equals(usuario.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existente = _usuarioAppService.AsQueryable().FirstOrDefault(f => f.Email.Equals(usuario.Email ?? string.Empty));

                if (existente is not null && existente.UsuarioId != usuario.UsuarioId)
                {
                    ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
            {
                if (!isAdmin && !_usuarioAppService.VerifyPassword(usuario, model.SenhaAtual ?? string.Empty))
                {
                    ModelState.AddModelError(nameof(model.SenhaAtual), "A senha atual informada não confere.");
                    return View(model);
                }

                usuario.Senha = model.NovaSenha;
            }

            usuario.Nome = model.Nome.Trim();
            usuario.Email = model.Email.Trim();
            usuario.Ativo = model.Ativo;

            if (isAdmin && model.PermiteAlterarPerfil && Enum.TryParse(model.Perfil, out Enums.PerfilUsuario perfil))
                usuario.Perfil = perfil;

            _usuarioAppService.Update(usuario);

            TempData["Sucesso"] = "Alterações salvas com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioAppService.AsQueryable().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, usuario.Email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            var model = new UsuarioDetalheViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                Ativo = usuario.Ativo,
                CriadoEm = usuario.DataCadastro,
                AcoesRecentes = Array.Empty<string>()
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id, string email)
        {
            var isAdmin = User.IsInRole(Enums.PerfilUsuario.Administrador.ToString());
            var isSelf = string.Equals(User.Identity?.Name, email, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isSelf)
                return Forbid();

            var usuario = await _usuarioAppService.AsTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return NotFound();

            _usuarioAppService.Delete(usuario);

            TempData["Sucesso"] = "Usuário removido com sucesso.";
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
