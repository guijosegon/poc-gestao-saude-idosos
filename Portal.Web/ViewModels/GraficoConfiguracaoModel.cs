using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class GraficoConfiguracaoModel
    {
        public GraficoConfiguracaoPacienteModel? Paciente { get; set; }
        public GraficoConfiguracaoFormularioModel? Formulario { get; set; }
        public string? TituloEixoHorizontal { get; set; }
        public string? TituloEixoVertical { get; set; }
        public bool MostrarLegenda { get; set; } = true;
    }

    public class GraficoConfiguracaoPacienteModel
    {
        public Enums.GraficoPacienteCampo CampoCategoria { get; set; }
    }

    public class GraficoConfiguracaoFormularioModel
    {
        public int FormularioId { get; set; }
        public int CampoId { get; set; }
    }
}
