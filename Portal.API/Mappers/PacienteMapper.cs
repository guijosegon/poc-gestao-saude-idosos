using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Domain.Extensions;

namespace GestaoSaudeIdosos.API.Mappers
{
    public static class PacienteMapper
    {
        public static Paciente ToEntity(this PacienteDto dto)
        {
            return new Paciente
            {
                PacienteId = dto.PacienteId ?? 0,
                NomeCompleto = dto.Nome,
                DataNascimento = (dto.DataNascimento ?? DateTime.UtcNow).EnsureUtc(),
                ResponsavelId = dto.ResponsavelId,
                Idade = CalcularIdade(dto.DataNascimento.EnsureUtc())
            };
        }

        public static void UpdateEntity(this Paciente entity, PacienteDto dto)
        {
            entity.NomeCompleto = dto.Nome;
            if (dto.DataNascimento.HasValue)
            {
                var dataNascimento = dto.DataNascimento.Value.EnsureUtc();
                entity.DataNascimento = dataNascimento;
                entity.Idade = CalcularIdade(dataNascimento);
            }

            entity.ResponsavelId = dto.ResponsavelId;
        }

        public static PacienteDto ToDto(this Paciente entity)
        {
            return new PacienteDto
            {
                PacienteId = entity.PacienteId,
                Nome = entity.NomeCompleto,
                DataNascimento = entity.DataNascimento,
                ResponsavelId = entity.ResponsavelId
            };
        }

        private static int CalcularIdade(DateTime? dataNascimento)
        {
            if (!dataNascimento.HasValue)
                return 0;

            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Value.Year;

            if (dataNascimento.Value.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }
    }
}
