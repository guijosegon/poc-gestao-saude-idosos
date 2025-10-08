using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteListItemViewModel
    {
        public int PacienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public int Idade { get; set; }
        public string? Responsavel { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
