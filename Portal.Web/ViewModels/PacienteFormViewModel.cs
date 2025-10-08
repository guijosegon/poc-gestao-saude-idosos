using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteFormViewModel
    {
        public int? PacienteId { get; set; }

        [Required(ErrorMessage = "O nome do paciente é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Responsável")]
        [Required(ErrorMessage = "Selecione um responsável.")]
        public int? ResponsavelId { get; set; }

        public IEnumerable<SelectListItem> Responsaveis { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
