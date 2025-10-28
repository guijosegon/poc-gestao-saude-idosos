using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioDetalheViewModel
    {
        public int FormularioId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string? Responsavel { get; set; }
        public DateTime DataCadastro { get; set; }
        public IEnumerable<FormularioCampoResumoViewModel> Campos { get; set; } = Array.Empty<FormularioCampoResumoViewModel>();
        public IEnumerable<FormularioResultadoResumoViewModel> Aplicacoes { get; set; } = Array.Empty<FormularioResultadoResumoViewModel>();
    }
}
