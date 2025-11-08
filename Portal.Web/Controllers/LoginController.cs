using System.Security.Claims;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoSaudeIdosos.Web.Options;
using Microsoft.Extensions.Options;

namespace GestaoSaudeIdosos.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioAppService _service;
        private readonly AdminUserOptions _adminUserOptions;

        public LoginController(IUsuarioAppService service, IOptions<AdminUserOptions> adminUserOptions)
        {
            _service = service;
            _adminUserOptions = adminUserOptions.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index() => View(new LoginViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim() ?? string.Empty;
            var adminEmail = _adminUserOptions.Email?.Trim() ?? string.Empty;
            var isAdmin =
                !string.IsNullOrWhiteSpace(adminEmail) &&
                string.Equals(email, adminEmail, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(model.Senha, _adminUserOptions.Password);

            var usuario = isAdmin
                ? new Domain.Entities.Usuario() { UsuarioId = 0, NomeCompleto = _adminUserOptions.Name, Email = _adminUserOptions.Email, Ativo = true }
                : _service.AsQueryable().FirstOrDefault(f => f.Email == email);

            if (usuario is null || (usuario is not null && !usuario.Ativo))
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail está inválido ou não está autorizado. Solicite acesso ao administrador.");
                return View(model);
            }

            if (!_service.VerifyPassword(usuario, model.Senha) && !isAdmin)
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail está inválido ou não está autorizado. Solicite acesso ao administrador.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NomeCompleto),
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