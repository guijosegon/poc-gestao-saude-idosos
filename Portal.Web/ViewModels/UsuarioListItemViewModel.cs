using GestaoSaudeIdosos.Domain.Common.Helpers;
using System;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioListItemViewModel
    {
        public int UsuarioId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CpfRg { get; set; }
        public string? ImagemPerfil { get; set; }
        public Enums.PerfilUsuario Perfil { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
