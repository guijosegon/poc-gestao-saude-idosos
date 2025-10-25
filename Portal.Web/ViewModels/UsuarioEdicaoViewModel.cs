using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GestaoSaudeIdosos.Domain.Common.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioEdicaoViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um perfil.")]
        public string Perfil { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        [Display(Name = "Senha atual")]
        [DataType(DataType.Password)]
        public string? SenhaAtual { get; set; }

        [Display(Name = "Nova senha")]
        [DataType(DataType.Password)]
        [PasswordComplexity]
        public string? NovaSenha { get; set; }

        [Display(Name = "Confirmar nova senha")]
        [DataType(DataType.Password)]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem.")]
        public string? ConfirmacaoSenha { get; set; }

        public bool PermiteAlterarPerfil { get; set; }
        public IEnumerable<SelectListItem> PerfisDisponiveis { get; set; } = new List<SelectListItem>();
    }
}
