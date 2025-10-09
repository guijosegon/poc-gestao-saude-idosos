using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class GraficosController : Controller
    {
        private readonly IGraficoAppService _graficoAppService;

        public GraficosController(IGraficoAppService graficoAppService)
        {
            _graficoAppService = graficoAppService;
        }

        public async Task<IActionResult> Index()
        {
            var graficos = await _graficoAppService.GetAllAsync();
            var model = graficos
                .Select(g => new GraficoListItemViewModel
                {
                    GraficoId = g.GraficoId,
                    Descricao = g.Descricao
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var grafico = await _graficoAppService.GetByIdAsync(id);
            if (grafico is null)
                return NotFound();

            var model = new GraficoDetalheViewModel
            {
                GraficoId = grafico.GraficoId,
                Descricao = grafico.Descricao
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new GraficoFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GraficoFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var grafico = new Grafico
            {
                Descricao = model.Descricao.Trim()
            };

            await _graficoAppService.CreateAsync(grafico);

            TempData["Sucesso"] = "Gráfico cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var grafico = await _graficoAppService.GetByIdAsync(id);
            if (grafico is null)
                return NotFound();

            var model = new GraficoFormViewModel
            {
                GraficoId = grafico.GraficoId,
                Descricao = grafico.Descricao
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GraficoFormViewModel model)
        {
            if (model.GraficoId is null || model.GraficoId != id)
                ModelState.AddModelError(string.Empty, "Não foi possível localizar o gráfico informado.");

            if (!ModelState.IsValid)
                return View(model);

            var grafico = await _graficoAppService.GetByIdAsync(id);
            if (grafico is null)
                return NotFound();

            grafico.Descricao = model.Descricao.Trim();
            _graficoAppService.Update(grafico);

            TempData["Sucesso"] = "Gráfico atualizado com sucesso.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grafico = await _graficoAppService.GetByIdAsync(id);
            if (grafico is null)
                return NotFound();

            var model = new GraficoDetalheViewModel
            {
                GraficoId = grafico.GraficoId,
                Descricao = grafico.Descricao
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var grafico = await _graficoAppService.GetByIdAsync(id);
            if (grafico is null)
                return NotFound();

            _graficoAppService.Delete(grafico);

            TempData["Sucesso"] = "Gráfico removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
