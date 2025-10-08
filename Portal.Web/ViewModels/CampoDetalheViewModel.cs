using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class CampoDetalheViewModel
    {
        public int CampoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? TextoAjuda { get; set; }
        public IEnumerable<string> Opcoes { get; set; } = Array.Empty<string>();
        public bool Ativo { get; set; }
        public string? CriadoPor { get; set; }
        public DateTimeOffset DataCadastro { get; set; }
        public IEnumerable<string> FormulariosUtilizacao { get; set; } = Array.Empty<string>();
    }
}
