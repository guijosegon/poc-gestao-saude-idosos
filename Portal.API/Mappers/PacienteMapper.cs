using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.Domain.Entities;
using System;

namespace GestaoSaudeIdosos.API.Mappers
{
    public static class PacienteMapper
    {
        public static Paciente ToEntity(this PacienteDto dto)
        {
            return new Paciente
            {
                PacienteId = dto.PacienteId ?? 0,
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento ?? DateTime.UtcNow,
                ResponsavelId = dto.ResponsavelId,
                Idade = CalcularIdade(dto.DataNascimento)
            };
        }

        public static void UpdateEntity(this Paciente entity, PacienteDto dto)
        {
            entity.Nome = dto.Nome;
            if (dto.DataNascimento.HasValue)
            {
                entity.DataNascimento = dto.DataNascimento.Value;
                entity.Idade = CalcularIdade(dto.DataNascimento);
            }

            entity.ResponsavelId = dto.ResponsavelId;
        }

        public static PacienteDto ToDto(this Paciente entity)
        {
            return new PacienteDto
            {
                PacienteId = entity.PacienteId,
                Nome = entity.Nome,
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
