using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioDetalheViewModel
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public IEnumerable<string> AcoesRecentes { get; set; } = Array.Empty<string>();
    }
}
