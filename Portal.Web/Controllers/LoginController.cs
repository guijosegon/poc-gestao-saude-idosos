using System.Security.Claims;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoSaudeIdosos.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioAppService _service;

        public LoginController(IUsuarioAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim() ?? string.Empty;
            var usuario = _service.AsQueryable().FirstOrDefault(f => f.Email == email);

            if (usuario is null)
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail não está autorizado. Solicite acesso ao administrador.");
                return View(model);
            }

            if (!usuario.Ativo)
            {
                ModelState.AddModelError(string.Empty, "Usuário inativo. Entre em contato com o administrador.");
                return View(model);
            }

            if (!_service.VerifyPassword(usuario, model.Senha))
            {
                ModelState.AddModelError(nameof(model.Senha), "Senha inválida.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Portal.Auth");

            return RedirectToAction(nameof(Index), "Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
