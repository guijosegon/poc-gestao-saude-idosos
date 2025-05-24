using Microsoft.AspNetCore.Mvc;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GestaoSaudeIdosos.Application.Interfaces;

public class LoginController : Controller
{
    private readonly IUsuarioAppService _service;

    public LoginController(IUsuarioAppService service)
    {
        _service = service;
    }

    public IActionResult Index(string? returnUrl)
    {
        ViewData["Title"] = "Login";
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = _service.GetByEmail(model.Email);

        if (usuario is null)
        {
            ModelState.AddModelError(nameof(model.Email), "Este e-mail não está cadastrado.");
            return View(model);
        }

        if (model.Senha != usuario.Senha)
        {
            ModelState.AddModelError(nameof(model.Senha), "Senha incorreta.");
            return View(model);
        }

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Email) };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = model.LembrarMe };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }
}