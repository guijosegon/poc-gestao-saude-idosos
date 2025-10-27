using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GestaoSaudeIdosos.Domain.Common.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioEdicaoViewModel
    {
        public int UsuarioId { get; set; }

        [Display(Name = "Nome completo")]
        [Required(ErrorMessage = "Informe o nome completo.")]
        [StringLength(120, ErrorMessage = "O nome completo deve ter no máximo 120 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "CPF/RG")]
        [StringLength(40, ErrorMessage = "O CPF/RG deve ter no máximo 40 caracteres.")]
        public string? CpfRg { get; set; }

        [Display(Name = "Foto (URL)")]
        [StringLength(250, ErrorMessage = "A URL da imagem deve ter no máximo 250 caracteres.")]
        public string? ImagemPerfil { get; set; }

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
