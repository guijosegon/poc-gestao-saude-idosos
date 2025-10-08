using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class DashboardConfigViewModel
    {
        [Display(Name = "Tema do portal")]
        public string Tema { get; set; } = "Claro";

        [Display(Name = "Período padrão")]
        public string PeriodoPadrao { get; set; } = "Últimos 30 dias";

        [Display(Name = "Mostrar alertas críticos")]
        public bool MostrarAlertas { get; set; } = true;

        [Display(Name = "Resumo de atendimentos")]
        public bool MostrarResumoAtendimentos { get; set; } = true;

        public IList<int> GraficosSelecionados { get; set; } = new List<int>();

        public IEnumerable<DashboardGraficoViewModel> GraficosDisponiveis { get; set; } = new List<DashboardGraficoViewModel>();
        public IEnumerable<string> FiltrosFavoritos { get; set; } = new List<string>();
        public IEnumerable<SelectListItem> OpcoesPeriodo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TemasDisponiveis { get; set; } = new List<SelectListItem>();
    }

    public class DashboardGraficoViewModel
    {
        public int GraficoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
