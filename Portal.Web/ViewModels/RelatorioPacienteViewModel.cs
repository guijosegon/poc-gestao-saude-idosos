using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class RelatorioPacienteViewModel
    {
        public string PacienteNome { get; set; } = string.Empty;
        public string NivelRisco { get; set; } = string.Empty;
        public string Resumo { get; set; } = string.Empty;
        public string Sugestao { get; set; } = string.Empty;
        public DateTime? UltimaAtualizacao { get; set; }
    }
}
