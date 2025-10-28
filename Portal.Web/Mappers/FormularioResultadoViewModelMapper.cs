using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class FormularioResultadoViewModelMapper
    {
        public static FormularioAplicacaoViewModel CriarAplicacaoViewModel(Formulario formulario, Paciente paciente, string? abaOrigem, string? urlOrigem)
        {
            if (formulario is null)
                throw new ArgumentNullException(nameof(formulario));

            if (paciente is null)
                throw new ArgumentNullException(nameof(paciente));

            var campos = formulario.Campos?
                .OrderBy(fc => fc.Ordem)
                .Select(fc => new FormularioAplicacaoCampoViewModel
                {
                    FormularioCampoId = fc.FormularioCampoId,
                    CampoId = fc.CampoId,
                    CampoDescricao = fc.Campo?.Descricao ?? string.Empty,
                    Tipo = fc.Campo?.Tipo ?? Enums.TipoCampo.Texto,
                    Obrigatorio = fc.Obrigatorio,
                    Ordem = fc.Ordem,
                    TextoAjuda = fc.Campo?.TextoAjuda,
                    Opcoes = fc.Campo?.Opcoes?.AsReadOnly() ?? Array.Empty<string>(),
                    Valor = null,
                    ValorBooleano = false
                })
                .ToList() ?? new List<FormularioAplicacaoCampoViewModel>();

            return new FormularioAplicacaoViewModel
            {
                FormularioId = formulario.FormularioId,
                PacienteId = paciente.PacienteId,
                FormularioDescricao = formulario.Descricao,
                PacienteNome = paciente.NomeCompleto,
                AbaOrigem = abaOrigem,
                UrlOrigem = urlOrigem,
                Campos = campos
            };
        }

        public static FormularioAplicacaoViewModel CriarAplicacaoViewModel(Formulario formulario, Paciente paciente, FormularioAplicacaoInputModel modelo)
        {
            var viewModel = CriarAplicacaoViewModel(formulario, paciente, modelo.AbaOrigem, modelo.UrlOrigem);

            var valores = modelo.Campos.ToDictionary(c => c.FormularioCampoId, c => c);

            foreach (var campo in viewModel.Campos)
            {
                if (valores.TryGetValue(campo.FormularioCampoId, out var valor))
                {
                    campo.Valor = valor.Valor;
                    campo.ValorBooleano = valor.ValorBooleano;
                }
            }

            return viewModel;
        }

        public static FormularioResultadoDetalheViewModel CriarDetalheViewModel(FormularioResultado resultado, string? abaOrigem, string? urlOrigem)
        {
            var campos = new List<FormularioResultadoCampoDetalheViewModel>();

            if (resultado.Formulario?.Campos != null && resultado.Valores != null)
            {
                var valoresPorCampo = resultado.Valores.ToDictionary(v => v.FormularioCampoId, v => v);

                foreach (var formularioCampo in resultado.Formulario.Campos.OrderBy(fc => fc.Ordem))
                {
                    valoresPorCampo.TryGetValue(formularioCampo.FormularioCampoId, out var valor);
                    var campoEntidade = formularioCampo.Campo;
                    var tipo = campoEntidade?.Tipo ?? Enums.TipoCampo.Texto;
                    var exibicao = ObterValorFormatado(valor?.Valor, tipo);

                    campos.Add(new FormularioResultadoCampoDetalheViewModel
                    {
                        Campo = campoEntidade?.Descricao ?? string.Empty,
                        Tipo = tipo,
                        Obrigatorio = formularioCampo.Obrigatorio,
                        ValorApresentacao = exibicao
                    });
                }
            }

            return new FormularioResultadoDetalheViewModel
            {
                FormularioResultadoId = resultado.FormularioResultadoId,
                Formulario = resultado.Formulario?.Descricao ?? string.Empty,
                Paciente = resultado.Paciente?.NomeCompleto ?? string.Empty,
                DataAplicacao = resultado.DataPreenchimento,
                ResponsavelAplicacao = resultado.UsuarioAplicacao?.NomeCompleto,
                Campos = campos,
                AbaOrigem = abaOrigem,
                UrlOrigem = urlOrigem
            };
        }

        public static IEnumerable<FormularioResultadoResumoViewModel> MapearParaResumo(IEnumerable<FormularioResultado> resultados)
        {
            return resultados
                .OrderByDescending(r => r.DataPreenchimento)
                .Select(r => new FormularioResultadoResumoViewModel
                {
                    FormularioResultadoId = r.FormularioResultadoId,
                    Formulario = r.Formulario?.Descricao ?? string.Empty,
                    Paciente = r.Paciente?.NomeCompleto,
                    DataAplicacao = r.DataPreenchimento,
                    Responsavel = r.UsuarioAplicacao?.NomeCompleto,
                    Destaque = ExtrairDestaque(r)
                })
                .ToList();
        }

        private static string ObterValorFormatado(string? valor, Enums.TipoCampo tipo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return "—";
            }

            return tipo switch
            {
                Enums.TipoCampo.Data => FormatDate(valor),
                Enums.TipoCampo.DataHora => FormatDateTime(valor),
                Enums.TipoCampo.Checkbox => valor.Equals("true", StringComparison.OrdinalIgnoreCase) ? "Sim" : "Não",
                _ => valor
            };
        }

        private static string FormatDate(string valor)
        {
            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data))
            {
                return data.ToLocalTime().ToString("dd/MM/yyyy");
            }

            return valor;
        }

        private static string FormatDateTime(string valor)
        {
            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data))
            {
                return data.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            }

            return valor;
        }

        private static string? ExtrairDestaque(FormularioResultado resultado)
        {
            if (resultado.Valores is null || resultado.Valores.Count == 0)
            {
                return null;
            }

            var valor = resultado.Valores
                .OrderBy(v => v.FormularioCampo?.Ordem ?? int.MaxValue)
                .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v.Valor));

            if (valor is null)
            {
                return null;
            }

            var tipo = valor.Campo?.Tipo ?? Enums.TipoCampo.Texto;
            return ObterValorFormatado(valor.Valor, tipo);
        }
    }
}
