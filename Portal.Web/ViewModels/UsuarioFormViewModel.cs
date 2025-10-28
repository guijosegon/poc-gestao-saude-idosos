using System.ComponentModel.DataAnnotations;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Common.Validation;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioFormViewModel
    {
        public int? UsuarioId { get; set; }

        [Display(Name = "Nome completo")]
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome completo deve ter no máximo 120 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "CPF/RG")]
        [StringLength(40, ErrorMessage = "O CPF/RG deve ter no máximo 40 caracteres.")]
        public string? CpfRg { get; set; }

        public string? ImagemPerfil { get; set; }

        [Display(Name = "Foto do perfil")]
        public IFormFile? ImagemPerfilArquivo { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [PasswordComplexity]
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
