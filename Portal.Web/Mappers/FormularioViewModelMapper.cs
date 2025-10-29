using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class FormularioViewModelMapper
    {
        public static Expression<Func<Formulario, FormularioListItemViewModel>> ToListItem => formulario => new FormularioListItemViewModel
        {
            FormularioId = formulario.FormularioId,
            Descricao = formulario.Descricao,
            QuantidadeCampos = formulario.Campos != null ? formulario.Campos.Count : 0,
            QuantidadePacientes = formulario.Pacientes != null ? formulario.Pacientes.Count : 0,
            Ativo = formulario.Ativo,
            DataCadastro = formulario.DataCadastro
        };

        public static Expression<Func<Formulario, FormularioDetalheViewModel>> ToDetail => formulario => new FormularioDetalheViewModel
        {
            FormularioId = formulario.FormularioId,
            Descricao = formulario.Descricao,
            Ativo = formulario.Ativo,
            Responsavel = formulario.Usuario != null ? formulario.Usuario.NomeCompleto : null,
            DataCadastro = formulario.DataCadastro,
            Campos = formulario.Campos != null
                ? formulario.Campos
                    .OrderBy(c => c.Ordem)
                    .Select(c => new FormularioCampoResumoViewModel
                    {
                        NomeCampo = c.Campo != null ? c.Campo.Descricao : string.Empty,
                        Tipo = c.Campo != null ? c.Campo.Tipo.ToString() : string.Empty,
                        Obrigatorio = c.Obrigatorio,
                        Ordem = c.Ordem
                    })
                    .ToList()
                : new List<FormularioCampoResumoViewModel>()
        };

        public static FormularioFormViewModel ToFormViewModel(this Formulario formulario, IEnumerable<SelectListItem> camposDisponiveis)
        {
            return new FormularioFormViewModel
            {
                FormularioId = formulario.FormularioId,
                Descricao = formulario.Descricao,
                Ativo = formulario.Ativo,
                CamposSelecionados = formulario.Campos?.Select(c => c.CampoId).ToList() ?? new List<int>(),
                CamposDisponiveis = camposDisponiveis
            };
        }

        public static FormularioDetalheViewModel ToDetailModel(this Formulario formulario)
        {
            return new FormularioDetalheViewModel
            {
                FormularioId = formulario.FormularioId,
                Descricao = formulario.Descricao,
                Ativo = formulario.Ativo,
                Responsavel = formulario.Usuario?.NomeCompleto,
                DataCadastro = formulario.DataCadastro,
                Campos = formulario.Campos != null
                    ? formulario.Campos
                        .OrderBy(c => c.Ordem)
                        .Select(c => new FormularioCampoResumoViewModel
                        {
                            NomeCampo = c.Campo != null ? c.Campo.Descricao : string.Empty,
                            Tipo = c.Campo != null ? c.Campo.Tipo.ToString() : string.Empty,
                            Obrigatorio = c.Obrigatorio,
                            Ordem = c.Ordem
                        })
                        .ToList()
                    : new List<FormularioCampoResumoViewModel>(),
                Aplicacoes = formulario.Resultados != null
                    ? FormularioResultadoViewModelMapper.MapearParaResumo(formulario.Resultados).ToList()
                    : new List<FormularioResultadoResumoViewModel>()
            };
        }

        public static Formulario ToEntity(this FormularioFormViewModel model, int? usuarioId)
        {
            return new Formulario
            {
                FormularioId = model.FormularioId ?? 0,
                Descricao = model.Descricao.Trim(),
                Ativo = model.Ativo,
                UsuarioId = usuarioId
            };
        }

        public static void ApplyToEntity(this FormularioFormViewModel model, Formulario formulario)
        {
            formulario.Descricao = model.Descricao.Trim();
            formulario.Ativo = model.Ativo;
        }
    }
}