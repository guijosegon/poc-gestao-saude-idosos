using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.Domain.Entities;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.API.Mappers
{
    public static class UsuarioMapper
    {
        public static Expression<Func<Usuario, Usuario>> Expression(IMap<Usuario, Usuario> _)
        {
            Expression<Func<Usuario, Usuario>> b = a => new Usuario
            {
                UsuarioId = a.UsuarioId,
                Nome = a.Nome,
                Email = a.Email,
                Senha = a.Senha,
                Perfil = a.Perfil,
                Ativo = a.Ativo,
                DataCadastro = a.DataCadastro,
                Chave = a.Chave
            };

            return b;
        }

        public static Usuario ToEntity(this UsuarioDto dto)
        {
            return new Usuario
            {
                UsuarioId = dto.UsuarioId ?? 0,
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Perfil = dto.Perfil,
                Ativo = dto.Ativo
            };
        }

        public static void UpdateEntity(this Usuario entity, UsuarioDto dto)
        {
            entity.Nome = dto.Nome;
            entity.Email = dto.Email;
            entity.Perfil = dto.Perfil;
            entity.Ativo = dto.Ativo;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                entity.Senha = dto.Senha;
        }

        public static UsuarioDto ToDto(this Usuario entity)
        {
            return new UsuarioDto
            {
                UsuarioId = entity.UsuarioId,
                Nome = entity.Nome,
                Email = entity.Email,
                Senha = entity.Senha,
                Perfil = entity.Perfil,
                Ativo = entity.Ativo
            };
        }
    }
}
