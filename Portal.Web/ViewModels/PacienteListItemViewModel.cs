using System;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Web.Extensions;

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
        public string? CondicoesCronicas { get; set; }
        public string? HistoricoCirurgico { get; set; }
        public Enums.RiscoQuedaPaciente RiscoQueda { get; set; } = Enums.RiscoQuedaPaciente.SemRisco;
        public Enums.MobilidadePaciente Mobilidade { get; set; } = Enums.MobilidadePaciente.Independente;
        public string? DietasRestricoes { get; set; }

        public string RiscoQuedaDescricao => RiscoQueda.GetDisplayName();
        public string MobilidadeDescricao => Mobilidade.GetDisplayName();
    }
}
