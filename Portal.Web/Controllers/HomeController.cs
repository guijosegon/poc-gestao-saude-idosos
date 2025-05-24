using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        ViewBag.UsuarioNome = "Guilherme"; // depois puxaremos do login real
        return View();
    }
}