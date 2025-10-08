using System.ComponentModel.DataAnnotations;
using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.API.DTOs
{
    public class UsuarioDto
    {
        public int? UsuarioId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Perfil é obrigatório")]
        public Enums.PerfilUsuario Perfil { get; set; } = Enums.PerfilUsuario.Comum;

        public bool Ativo { get; set; } = true;
    }
}
