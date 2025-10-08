using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioListItemViewModel
    {
        public int FormularioId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QuantidadeCampos { get; set; }
        public int QuantidadePacientes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
