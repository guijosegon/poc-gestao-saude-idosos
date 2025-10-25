using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using System;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class UsuarioViewModelMapper
    {
        public static Expression<Func<Usuario, UsuarioListItemViewModel>> ToListItem => usuario => new UsuarioListItemViewModel
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro
        };

        public static Expression<Func<Usuario, UsuarioDetalheViewModel>> ToDetail => usuario => new UsuarioDetalheViewModel
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            Ativo = usuario.Ativo,
            CriadoEm = usuario.DataCadastro
        };

        public static Expression<Func<Usuario, UsuarioEdicaoViewModel>> ToEditForm => usuario => new UsuarioEdicaoViewModel
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            Ativo = usuario.Ativo
        };

        public static Usuario ToEntity(this UsuarioFormViewModel model)
        {
            return new Usuario
            {
                UsuarioId = model.UsuarioId ?? 0,
                Nome = model.Nome.Trim(),
                Email = model.Email.Trim(),
                Senha = model.Senha,
                Perfil = model.Perfil,
                Ativo = model.Ativo
            };
        }

        public static void ApplyToEntity(this UsuarioEdicaoViewModel model, Usuario entity, bool alterarPerfil)
        {
            entity.Nome = model.Nome.Trim();
            entity.Email = model.Email.Trim();
            entity.Ativo = model.Ativo;

            if (alterarPerfil && !string.IsNullOrWhiteSpace(model.Perfil))
            {
                entity.Perfil = Enum.Parse<Enums.PerfilUsuario>(model.Perfil, true);
            }
        }
    }
}
