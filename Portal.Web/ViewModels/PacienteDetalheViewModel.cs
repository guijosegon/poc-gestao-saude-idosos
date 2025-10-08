using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteDetalheViewModel
    {
        public int PacienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public DateTime UltimaAtualizacao { get; set; }
        public IEnumerable<PacienteFormularioResultadoViewModel> FormulariosRecentes { get; set; } = Array.Empty<PacienteFormularioResultadoViewModel>();
    }

    public class PacienteFormularioResultadoViewModel
    {
        public string Formulario { get; set; } = string.Empty;
        public DateTime DataAplicacao { get; set; }
        public string ResponsavelAplicacao { get; set; } = string.Empty;
        public string Destaque { get; set; } = string.Empty;
    }
}
