using GestaoSaudeIdosos.Application.Interfaces;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GestaoSaudeIdosos.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IGraficoAppService _graficoAppService;

        public DashboardController(IGraficoAppService graficoAppService)
        {
            _graficoAppService = graficoAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CriarDashboardAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DashboardConfigViewModel model)
        {
            var referencia = await CriarDashboardAsync();
            model.GraficosDisponiveis = referencia.GraficosDisponiveis;
            model.OpcoesPeriodo = referencia.OpcoesPeriodo;
            model.TemasDisponiveis = referencia.TemasDisponiveis;
            model.FiltrosFavoritos = referencia.FiltrosFavoritos;
            model.GraficosSelecionados ??= new List<int>();

            if (!ModelState.IsValid)
                return View(model);

            var selecionados = model.GraficosSelecionados.Distinct().ToHashSet();
            var graficos = await _graficoAppService.AsQueryable().ToListAsync();

            foreach (var grafico in graficos)
            {
                var exibir = selecionados.Contains(grafico.GraficoId);

                if (grafico.ExibirNoPortal != exibir)
                {
                    grafico.ExibirNoPortal = exibir;
                    _graficoAppService.Update(grafico);
                }
            }

            model.GraficosSelecionados = selecionados.ToList();

            TempData["Sucesso"] = "Preferências do dashboard atualizadas.";
            return View(model);
        }

        private async Task<DashboardConfigViewModel> CriarDashboardAsync()
        {
            var graficos = await _graficoAppService.AsQueryable().ToListAsync();

            var opcoesPeriodo = new List<SelectListItem>
            {
                new SelectListItem { Text = "Últimos 7 dias", Value = "7" },
                new SelectListItem { Text = "Últimos 30 dias", Value = "30" },
                new SelectListItem { Text = "Últimos 90 dias", Value = "90" }
            };

            var temas = new List<SelectListItem>
            {
                new SelectListItem { Text = "Claro", Value = "Claro" },
                new SelectListItem { Text = "Escuro", Value = "Escuro" }
            };

            return new DashboardConfigViewModel
            {
                TemasDisponiveis = temas,
                OpcoesPeriodo = opcoesPeriodo,
                FiltrosFavoritos = new[] { "Pacientes em risco", "Formulários pendentes" },
                GraficosDisponiveis = graficos
                    .Select(g => new DashboardGraficoViewModel
                    {
                        GraficoId = g.GraficoId,
                        Descricao = g.Descricao
                    })
                    .ToList(),
                GraficosSelecionados = graficos
                    .Where(g => g.ExibirNoPortal)
                    .Select(g => g.GraficoId)
                    .ToList()
            };
        }
    }
}
