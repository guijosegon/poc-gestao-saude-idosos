using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioFormViewModel
    {
        public int? FormularioId { get; set; }

        [Required(ErrorMessage = "Informe a descrição do formulário.")]
        [MaxLength(150, ErrorMessage = "Use no máximo 150 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Formulário ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Campos associados")]
        [Required(ErrorMessage = "Selecione ao menos um campo.")]
        public IList<int> CamposSelecionados { get; set; } = new List<int>();

        public IEnumerable<SelectListItem> CamposDisponiveis { get; set; } = new List<SelectListItem>();
    }
}
