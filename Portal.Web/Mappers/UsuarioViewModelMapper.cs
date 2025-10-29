using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Entities;
using GestaoSaudeIdosos.Web.ViewModels;
using System.Linq.Expressions;

namespace GestaoSaudeIdosos.Web.Mappers
{
    public static class UsuarioViewModelMapper
    {
        public static Expression<Func<Usuario, UsuarioListItemViewModel>> ToListItem => usuario => new UsuarioListItemViewModel
        {
            UsuarioId = usuario.UsuarioId,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email,
            CpfRg = usuario.CpfRg,
            ImagemPerfil = usuario.ImagemPerfil,
            Perfil = usuario.Perfil,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro
        };

        public static Expression<Func<Usuario, UsuarioDetalheViewModel>> ToDetail => usuario => new UsuarioDetalheViewModel
        {
            UsuarioId = usuario.UsuarioId,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email,
            CpfRg = usuario.CpfRg,
            ImagemPerfil = usuario.ImagemPerfil,
            Perfil = usuario.Perfil.ToString(),
            Ativo = usuario.Ativo,
            CriadoEm = usuario.DataCadastro
        };

        public static Expression<Func<Usuario, UsuarioEdicaoViewModel>> ToEditForm => usuario => new UsuarioEdicaoViewModel
        {
            UsuarioId = usuario.UsuarioId,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email,
            CpfRg = usuario.CpfRg,
            ImagemPerfil = usuario.ImagemPerfil,
            Perfil = usuario.Perfil.ToString(),
            Ativo = usuario.Ativo,
            RemoverImagemPerfil = false
        };

        public static Usuario ToEntity(this UsuarioFormViewModel model)
        {
            return new Usuario
            {
                UsuarioId = model.UsuarioId ?? 0,
                NomeCompleto = model.NomeCompleto.Trim(),
                Email = model.Email.Trim(),
                Senha = model.Senha,
                CpfRg = string.IsNullOrWhiteSpace(model.CpfRg) ? null : model.CpfRg.Trim(),
                ImagemPerfil = string.IsNullOrWhiteSpace(model.ImagemPerfil) ? null : model.ImagemPerfil.Trim(),
                Perfil = model.Perfil,
                Ativo = model.Ativo
            };
        }

        public static void ApplyToEntity(this UsuarioEdicaoViewModel model, Usuario entity, bool alterarPerfil)
        {
            entity.NomeCompleto = model.NomeCompleto.Trim();
            entity.Email = model.Email.Trim();
            entity.CpfRg = string.IsNullOrWhiteSpace(model.CpfRg) ? null : model.CpfRg.Trim();
            entity.ImagemPerfil = string.IsNullOrWhiteSpace(model.ImagemPerfil) ? null : model.ImagemPerfil.Trim();
            entity.Ativo = model.Ativo;

            if (alterarPerfil && !string.IsNullOrWhiteSpace(model.Perfil))
            {
                entity.Perfil = Enum.Parse<Enums.PerfilUsuario>(model.Perfil, true);
            }
        }
    }
}