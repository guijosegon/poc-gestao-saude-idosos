using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteListItemViewModel
    {
        public int PacienteId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string? CpfRg { get; set; }
        public string? ImagemPerfil { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Idade { get; set; }
        public string? Responsavel { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
