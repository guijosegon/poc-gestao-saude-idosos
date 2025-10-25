using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Extensions;
using GestaoSaudeIdosos.Web.ViewModels;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class PacienteViewModelMapper
    {
        public static Expression<Func<Paciente, PacienteListItemViewModel>> ToListItem => paciente => new PacienteListItemViewModel
        {
            PacienteId = paciente.PacienteId,
            Nome = paciente.Nome,
            DataNascimento = paciente.DataNascimento,
            Idade = paciente.Idade,
            Responsavel = paciente.Responsavel != null ? paciente.Responsavel.Nome : null,
            DataCadastro = paciente.DataCadastro
        };

        public static Expression<Func<Paciente, PacienteFormViewModel>> ToForm => paciente => new PacienteFormViewModel
        {
            PacienteId = paciente.PacienteId,
            Nome = paciente.Nome,
            DataNascimento = paciente.DataNascimento,
            ResponsavelId = paciente.ResponsavelId
        };

        public static PacienteDetalheViewModel ToDetalhe(this Paciente paciente)
        {
            return new PacienteDetalheViewModel
            {
                PacienteId = paciente.PacienteId,
                Nome = paciente.Nome,
                Idade = paciente.Idade,
                DataNascimento = paciente.DataNascimento,
                Responsavel = paciente.Responsavel?.Nome ?? string.Empty,
                UltimaAtualizacao = paciente.DataCadastro,
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
                Nome = model.Nome.Trim(),
                DataNascimento = dataNascimento,
                Idade = CalcularIdade(dataNascimento),
                ResponsavelId = model.ResponsavelId
            };
        }

        public static void ApplyToEntity(this PacienteFormViewModel model, Paciente entity)
        {
            if (!model.DataNascimento.HasValue)
                throw new InvalidOperationException("A data de nascimento do paciente é obrigatória.");

            var dataNascimento = model.DataNascimento.Value.EnsureUtc();

            entity.Nome = model.Nome.Trim();
            entity.DataNascimento = dataNascimento;
            entity.Idade = CalcularIdade(dataNascimento);
            entity.ResponsavelId = model.ResponsavelId;
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
    }
}
