using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteDetalheViewModel
    {
        public int PacienteId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string? CpfRg { get; set; }
        public string? ImagemPerfil { get; set; }
        public int Idade { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public DateTime UltimaAtualizacao { get; set; }
        public string Genero { get; set; } = string.Empty;
        public IEnumerable<string> CondicoesCronicas { get; set; } = Array.Empty<string>();
        public IEnumerable<string> HistoricoCirurgico { get; set; } = Array.Empty<string>();
        public string RiscoQueda { get; set; } = string.Empty;
        public string Mobilidade { get; set; } = string.Empty;
        public IEnumerable<string> DietasRestricoes { get; set; } = Array.Empty<string>();
        public IEnumerable<PacienteFormularioResultadoViewModel> FormulariosRecentes { get; set; } = Array.Empty<PacienteFormularioResultadoViewModel>();
    }

    public class PacienteFormularioResultadoViewModel
    {
        public int FormularioResultadoId { get; set; }
        public string Formulario { get; set; } = string.Empty;
        public DateTime DataAplicacao { get; set; }
        public string ResponsavelAplicacao { get; set; } = string.Empty;
        public string Destaque { get; set; } = string.Empty;
    }
}
