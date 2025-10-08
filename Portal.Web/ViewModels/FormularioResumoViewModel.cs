using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioResumoViewModel
    {
        public int FormularioId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int CamposTotais { get; set; }
        public int PacientesAssociados { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
