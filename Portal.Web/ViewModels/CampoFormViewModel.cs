using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class CampoFormViewModel
    {
        public int? CampoId { get; set; }

        [Required(ErrorMessage = "Informe a descrição do campo.")]
        [MaxLength(150, ErrorMessage = "Use no máximo 150 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Tipo do campo")]
        [Required(ErrorMessage = "Selecione um tipo de campo.")]
        public string Tipo { get; set; } = string.Empty;

        [Display(Name = "Texto de apoio")]
        [Required(ErrorMessage = "Informe o apoio do campo.")]
        [MaxLength(500, ErrorMessage = "Utilize no máximo 500 caracteres.")]
        public string? TextoAjuda { get; set; }

        [Display(Name = "Opções (uma por linha)")]
        public string? Opcoes { get; set; }

        [Display(Name = "Campo ativo")]
        public bool Ativo { get; set; } = true;

        public IEnumerable<SelectListItem> TiposCampo { get; set; } = new List<SelectListItem>();
    }
}
