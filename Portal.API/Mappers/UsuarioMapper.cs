using GestaoSaudeIdosos.API.DTOs;
using GestaoSaudeIdosos.Domain.Entities;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.API.Mappers
{
    public static class UsuarioMapper
    {
        public static Expression<Func<Usuario, Usuario>> Expression(IMap<Usuario, Usuario> _)
        {
            Expression<Func<Usuario, Usuario>> b = (a) => new Usuario
            {
                UsuarioId = a.UsuarioId,
                Nome = a.Nome,
                Email = a.Email,
                Senha = a.Senha,
                Perfil = a.Perfil
            };

            return b;
        }

        public static Usuario ToEntity(this UsuarioDto dto)
        {
            return new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
            };
        }

        public static UsuarioDto ToDto(this Usuario dto)
        {
            return new UsuarioDto
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
            };
        }
    }
}