using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Grafico
    {
        public int GraficoId { get; set; }
        public string Descricao { get; set; }
        public Enums.TipoOrigemGrafico Origem { get; set; }
        public Enums.TipoGrafico Tipo { get; set; }
        public string Configuracao { get; set; } = string.Empty;
        public bool ExibirNoPortal { get; set; }
        public int? FormularioId { get; set; }
        public virtual Formulario? Formulario { get; set; }
    }
}
