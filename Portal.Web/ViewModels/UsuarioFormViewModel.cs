using System.ComponentModel.DataAnnotations;
using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioFormViewModel
    {
        public int? UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a senha.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmacaoSenha { get; set; } = string.Empty;

        [Display(Name = "Perfil")]
        [Required(ErrorMessage = "Selecione um perfil.")]
        public Enums.PerfilUsuario Perfil { get; set; } = Enums.PerfilUsuario.Comum;

        public bool Ativo { get; set; } = true;
    }
}
