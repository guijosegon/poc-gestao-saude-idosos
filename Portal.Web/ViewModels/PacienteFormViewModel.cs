using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteFormViewModel
    {
        public int? PacienteId { get; set; }

        [Display(Name = "Nome completo")]
        [Required(ErrorMessage = "O nome do paciente é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "CPF/RG")]
        [StringLength(40, ErrorMessage = "O CPF/RG deve ter no máximo 40 caracteres.")]
        public string? CpfRg { get; set; }

        public string? ImagemPerfil { get; set; }

        [Display(Name = "Foto do perfil")]
        public IFormFile? ImagemPerfilArquivo { get; set; }

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Responsável")]
        [Required(ErrorMessage = "Selecione um responsável.")]
        public int? ResponsavelId { get; set; }

        public IEnumerable<SelectListItem> Responsaveis { get; set; } = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Condições crônicas e comorbidades")]
        public List<string> CondicoesCronicasSelecionadas { get; set; } = new();
        public IEnumerable<SelectListItem> CondicoesCronicasDisponiveis { get; set; } = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Histórico cirúrgico")]
        public List<string> HistoricoCirurgicoSelecionados { get; set; } = new();
        public IEnumerable<SelectListItem> HistoricoCirurgicoDisponiveis { get; set; } = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Risco de quedas")]
        public string? RiscoQuedaSelecionado { get; set; }
        public IEnumerable<SelectListItem> RiscoQuedasDisponiveis { get; set; } = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Mobilidade e auxílios")]
        public string? MobilidadeSelecionada { get; set; }
        public IEnumerable<SelectListItem> MobilidadeDisponivel { get; set; } = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Dietas e restrições alimentares")]
        public List<string> DietasSelecionadas { get; set; } = new();
        public IEnumerable<SelectListItem> DietasDisponiveis { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
