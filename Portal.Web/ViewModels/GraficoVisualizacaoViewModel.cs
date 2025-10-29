using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class GraficoVisualizacaoViewModel
    {
        public int GraficoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public Enums.TipoOrigemGrafico Origem { get; set; }
        public Enums.TipoGrafico Tipo { get; set; }
        public GraficoConfiguracaoModel Configuracao { get; set; } = new GraficoConfiguracaoModel();
    }
}
