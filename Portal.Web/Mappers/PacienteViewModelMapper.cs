using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Extensions;
using GestaoSaudeIdosos.Web.Extensions;
using GestaoSaudeIdosos.Web.ViewModels;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class PacienteViewModelMapper
    {
        public static Expression<Func<Paciente, PacienteListItemViewModel>> ToListItem => paciente => new PacienteListItemViewModel
        {
            PacienteId = paciente.PacienteId,
            NomeCompleto = paciente.NomeCompleto,
            CpfRg = paciente.CpfRg,
            ImagemPerfil = paciente.ImagemPerfil,
            DataNascimento = paciente.DataNascimento,
            Idade = paciente.Idade,
            Responsavel = paciente.Responsavel != null ? paciente.Responsavel.NomeCompleto : null,
            DataCadastro = paciente.DataCadastro
        };

        public static PacienteFormViewModel ToFormViewModel(this Paciente paciente)
        {
            return new PacienteFormViewModel
            {
                PacienteId = paciente.PacienteId,
                NomeCompleto = paciente.NomeCompleto,
                CpfRg = paciente.CpfRg,
                ImagemPerfil = paciente.ImagemPerfil,
                DataNascimento = paciente.DataNascimento,
                ResponsavelId = paciente.ResponsavelId,
                CondicoesCronicasSelecionadas = ConverterParaLista(paciente.CondicoesCronicas),
                HistoricoCirurgicoSelecionados = ConverterParaLista(paciente.HistoricoCirurgico),
                RiscoQuedasSelecionados = ConverterParaLista(paciente.RiscoQuedas),
                MobilidadeSelecionada = ConverterParaLista(paciente.MobilidadeAuxilios),
                DietasSelecionadas = ConverterParaLista(paciente.DietasRestricoes)
            };
        }

        public static PacienteDetalheViewModel ToDetalhe(this Paciente paciente)
        {
            return new PacienteDetalheViewModel
            {
                PacienteId = paciente.PacienteId,
                NomeCompleto = paciente.NomeCompleto,
                CpfRg = paciente.CpfRg,
                ImagemPerfil = paciente.ImagemPerfil,
                Idade = paciente.Idade,
                DataNascimento = paciente.DataNascimento,
                Responsavel = paciente.Responsavel?.NomeCompleto ?? string.Empty,
                UltimaAtualizacao = paciente.DataCadastro,
                CondicoesCronicas = ConverterParaDisplay<Enums.CondicaoCronicaPaciente>(paciente.CondicoesCronicas),
                HistoricoCirurgico = ConverterParaDisplay<Enums.HistoricoCirurgicoPaciente>(paciente.HistoricoCirurgico),
                RiscoQuedas = ConverterParaDisplay<Enums.RiscoQuedaPaciente>(paciente.RiscoQuedas),
                MobilidadeAuxilios = ConverterParaDisplay<Enums.MobilidadePaciente>(paciente.MobilidadeAuxilios),
                DietasRestricoes = ConverterParaDisplay<Enums.DietaRestricaoPaciente>(paciente.DietasRestricoes),
                FormulariosRecentes = Array.Empty<PacienteFormularioResultadoViewModel>()
            };
        }

        public static Paciente ToEntity(this PacienteFormViewModel model)
        {
            if (!model.DataNascimento.HasValue)
                throw new InvalidOperationException("A data de nascimento do paciente é obrigatória.");

            var dataNascimento = model.DataNascimento.Value.EnsureUtc();

            return new Paciente
            {
                PacienteId = model.PacienteId ?? 0,
                NomeCompleto = model.NomeCompleto.Trim(),
                CpfRg = string.IsNullOrWhiteSpace(model.CpfRg) ? null : model.CpfRg.Trim(),
                ImagemPerfil = string.IsNullOrWhiteSpace(model.ImagemPerfil) ? null : model.ImagemPerfil.Trim(),
                DataNascimento = dataNascimento,
                Idade = CalcularIdade(dataNascimento),
                ResponsavelId = model.ResponsavelId,
                CondicoesCronicas = ConverterParaString(model.CondicoesCronicasSelecionadas),
                HistoricoCirurgico = ConverterParaString(model.HistoricoCirurgicoSelecionados),
                RiscoQuedas = ConverterParaString(model.RiscoQuedasSelecionados),
                MobilidadeAuxilios = ConverterParaString(model.MobilidadeSelecionada),
                DietasRestricoes = ConverterParaString(model.DietasSelecionadas)
            };
        }

        public static void ApplyToEntity(this PacienteFormViewModel model, Paciente entity)
        {
            if (!model.DataNascimento.HasValue)
                throw new InvalidOperationException("A data de nascimento do paciente é obrigatória.");

            var dataNascimento = model.DataNascimento.Value.EnsureUtc();

            entity.NomeCompleto = model.NomeCompleto.Trim();
            entity.CpfRg = string.IsNullOrWhiteSpace(model.CpfRg) ? null : model.CpfRg.Trim();
            entity.ImagemPerfil = string.IsNullOrWhiteSpace(model.ImagemPerfil) ? null : model.ImagemPerfil.Trim();
            entity.DataNascimento = dataNascimento;
            entity.Idade = CalcularIdade(dataNascimento);
            entity.ResponsavelId = model.ResponsavelId;
            entity.CondicoesCronicas = ConverterParaString(model.CondicoesCronicasSelecionadas);
            entity.HistoricoCirurgico = ConverterParaString(model.HistoricoCirurgicoSelecionados);
            entity.RiscoQuedas = ConverterParaString(model.RiscoQuedasSelecionados);
            entity.MobilidadeAuxilios = ConverterParaString(model.MobilidadeSelecionada);
            entity.DietasRestricoes = ConverterParaString(model.DietasSelecionadas);
        }

        private static int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.UtcNow.Date;
            var idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
            {
                idade--;
            }

            return Math.Max(idade, 0);
        }

        private static List<string> ConverterParaLista(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new List<string>();

            return valor
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string? ConverterParaString(IEnumerable<string>? valores)
        {
            if (valores is null)
                return null;

            var itens = valores
                .Select(v => v?.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return itens.Count == 0 ? null : string.Join(';', itens);
        }

        private static IEnumerable<string> ConverterParaDisplay<TEnum>(string? valor)
            where TEnum : struct, Enum
        {
            return ConverterParaLista(valor)
                .Select(item => Enum.TryParse<TEnum>(item, true, out var parsed)
                    ? parsed.GetDisplayName()
                    : item)
                .ToList();
        }
    }
}
