using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class CampoViewModelMapper
    {
        public static Expression<Func<Campo, CampoListItemViewModel>> ToListItem => campo => new CampoListItemViewModel
        {
            CampoId = campo.CampoId,
            Descricao = campo.Descricao,
            Tipo = campo.Tipo.ToString(),
            Responsavel = campo.Usuario != null ? campo.Usuario.Nome : null,
            DataCadastro = campo.DataCadastro,
            Ativo = campo.Ativo,
            FormulariosVinculados = campo.FormularioCampos != null ? campo.FormularioCampos.Count : 0
        };

        public static CampoFormViewModel ToFormViewModel(this Campo campo, IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> tiposCampo)
        {
            return new CampoFormViewModel
            {
                CampoId = campo.CampoId,
                Descricao = campo.Descricao,
                Tipo = campo.Tipo.ToString(),
                TextoAjuda = campo.TextoAjuda,
                Opcoes = campo.Opcoes is null ? string.Empty : string.Join(Environment.NewLine, campo.Opcoes),
                Ativo = campo.Ativo,
                TiposCampo = tiposCampo
            };
        }

        public static CampoDetalheViewModel ToDetail(this Campo campo, IEnumerable<Formulario> formularios)
        {
            return new CampoDetalheViewModel
            {
                CampoId = campo.CampoId,
                Descricao = campo.Descricao,
                Tipo = ObterDescricaoTipo(campo.Tipo),
                TextoAjuda = campo.TextoAjuda,
                Opcoes = campo.Opcoes,
                Ativo = campo.Ativo,
                CriadoPor = campo.Usuario?.Nome,
                DataCadastro = campo.DataCadastro,
                FormulariosUtilizacao = formularios
                    .Where(f => f.Campos.Any(fc => fc.CampoId == campo.CampoId))
                    .Select(f => f.Descricao)
                    .ToList()
            };
        }

        public static Campo ToEntity(this CampoFormViewModel model)
        {
            return new Campo
            {
                CampoId = model.CampoId ?? 0,
                Descricao = model.Descricao.Trim(),
                Tipo = Enum.Parse<Enums.TipoCampo>(model.Tipo, true),
                TextoAjuda = string.IsNullOrWhiteSpace(model.TextoAjuda) ? null : model.TextoAjuda.Trim(),
                Ativo = model.Ativo,
                Opcoes = ConverterOpcoes(model.Opcoes)
            };
        }

        public static void ApplyToEntity(this CampoFormViewModel model, Campo campo)
        {
            campo.Descricao = model.Descricao.Trim();
            campo.Tipo = Enum.Parse<Enums.TipoCampo>(model.Tipo, true);
            campo.TextoAjuda = string.IsNullOrWhiteSpace(model.TextoAjuda) ? null : model.TextoAjuda.Trim();
            campo.Ativo = model.Ativo;
            campo.Opcoes = ConverterOpcoes(model.Opcoes);
        }

        public static string ObterDescricaoTipo(Enums.TipoCampo tipo)
        {
            var member = typeof(Enums.TipoCampo).GetMember(tipo.ToString()).FirstOrDefault();
            var display = member?.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                .OfType<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                .FirstOrDefault();

            return display?.Name ?? tipo.ToString();
        }

        public static IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterTiposCampo()
        {
            return Enum.GetValues(typeof(Enums.TipoCampo))
                .Cast<Enums.TipoCampo>()
                .Select(tipo => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = tipo.ToString(),
                    Text = ObterDescricaoTipo(tipo)
                })
                .ToList();
        }

        private static List<string> ConverterOpcoes(string? opcoes)
        {
            if (string.IsNullOrWhiteSpace(opcoes))
                return new List<string>();

            return opcoes
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrEmpty(o))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
