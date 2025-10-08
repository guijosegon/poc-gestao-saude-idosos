using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class GraficoFormViewModel
    {
        public int? GraficoId { get; set; }

        [Required(ErrorMessage = "Informe a descrição do gráfico.")]
        [MaxLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
        public string Descricao { get; set; } = string.Empty;
    }
}
