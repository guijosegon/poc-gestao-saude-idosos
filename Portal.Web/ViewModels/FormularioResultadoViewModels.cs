using GestaoSaudeIdosos.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioAplicacaoViewModel
    {
        public int FormularioId { get; set; }
        public int PacienteId { get; set; }
        public string FormularioDescricao { get; set; } = string.Empty;
        public string PacienteNome { get; set; } = string.Empty;
        public string? AbaOrigem { get; set; }
        public string? UrlOrigem { get; set; }
        public IList<FormularioAplicacaoCampoViewModel> Campos { get; set; } = new List<FormularioAplicacaoCampoViewModel>();
    }

    public class FormularioAplicacaoCampoViewModel
    {
        public int FormularioCampoId { get; set; }
        public int CampoId { get; set; }
        public string CampoDescricao { get; set; } = string.Empty;
        public Enums.TipoCampo Tipo { get; set; }
        public bool Obrigatorio { get; set; }
        public int Ordem { get; set; }
        public string? TextoAjuda { get; set; }
        public IReadOnlyCollection<string> Opcoes { get; set; } = Array.Empty<string>();
        public string? Valor { get; set; }
        public bool ValorBooleano { get; set; }
    }

    public class FormularioAplicacaoInputModel
    {
        [Required]
        public int FormularioId { get; set; }

        [Required]
        public int PacienteId { get; set; }

        public string? AbaOrigem { get; set; }
        public string? UrlOrigem { get; set; }

        public IList<FormularioAplicacaoCampoInputModel> Campos { get; set; } = new List<FormularioAplicacaoCampoInputModel>();
    }

    public class FormularioAplicacaoCampoInputModel
    {
        [Required]
        public int FormularioCampoId { get; set; }

        [Required]
        public int CampoId { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;

        public bool Obrigatorio { get; set; }
        public string? Valor { get; set; }
        public bool ValorBooleano { get; set; }
    }

    public class FormularioResultadoDetalheViewModel
    {
        public int FormularioResultadoId { get; set; }
        public string Formulario { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public DateTime DataAplicacao { get; set; }
        public string? ResponsavelAplicacao { get; set; }
        public string? AbaOrigem { get; set; }
        public string? UrlOrigem { get; set; }
        public IEnumerable<FormularioResultadoCampoDetalheViewModel> Campos { get; set; } = Array.Empty<FormularioResultadoCampoDetalheViewModel>();
    }

    public class FormularioResultadoCampoDetalheViewModel
    {
        public string Campo { get; set; } = string.Empty;
        public Enums.TipoCampo Tipo { get; set; }
        public bool Obrigatorio { get; set; }
        public string ValorApresentacao { get; set; } = string.Empty;
    }

    public class FormularioAplicacaoSelecaoViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public IEnumerable<FormularioAplicacaoSelecaoItemViewModel> Itens { get; set; } = Array.Empty<FormularioAplicacaoSelecaoItemViewModel>();
    }

    public class FormularioAplicacaoSelecaoItemViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Complemento { get; set; }
    }

    public class FormularioResultadoResumoViewModel
    {
        public int FormularioResultadoId { get; set; }
        public string Formulario { get; set; } = string.Empty;
        public string? Paciente { get; set; }
        public DateTime DataAplicacao { get; set; }
        public string? Responsavel { get; set; }
        public string? Destaque { get; set; }
    }
}
