using System.Security.Claims;
using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IUsuarioAppService _service;

        public LoginController(IUsuarioAppService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _service.AsQueryable().FirstOrDefault(f => f.Email.Contains(model.Email));

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

            if (!string.Equals(model.Senha, usuario.Senha, StringComparison.Ordinal))
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}
