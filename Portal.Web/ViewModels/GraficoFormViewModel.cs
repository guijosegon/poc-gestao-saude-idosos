using GestaoSaudeIdosos.Domain.Common.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class GraficoFormViewModel
    {
        public int? GraficoId { get; set; }

        [Required(ErrorMessage = "Informe a descrição do gráfico.")]
        [MaxLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Origem dos dados")]
        public Enums.TipoOrigemGrafico Origem { get; set; } = Enums.TipoOrigemGrafico.Paciente;

        [Display(Name = "Tipo de gráfico")]
        public Enums.TipoGrafico Tipo { get; set; } = Enums.TipoGrafico.Coluna;

        [Display(Name = "Campo de agrupamento")]
        public Enums.GraficoPacienteCampo? PacienteCampoCategoria { get; set; }

        [Display(Name = "Formulário base")]
        public int? FormularioId { get; set; }

        [Display(Name = "Campo do formulário")]
        public int? FormularioCampoId { get; set; }

        [Display(Name = "Título do eixo horizontal")]
        [MaxLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
        public string? TituloEixoHorizontal { get; set; }

        [Display(Name = "Título do eixo vertical")]
        [MaxLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
        public string? TituloEixoVertical { get; set; }

        [Display(Name = "Exibir legenda")]
        public bool MostrarLegenda { get; set; } = true;

        [Display(Name = "Mostrar no portal principal")]
        public bool ExibirNoPortal { get; set; }

        public IEnumerable<SelectListItem> Origens { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TiposGrafico { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CamposPaciente { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Formularios { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CamposFormulario { get; set; } = new List<SelectListItem>();
    }
}
