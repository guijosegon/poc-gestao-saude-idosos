using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Extensions;
using System;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Usuario
    {
        public Usuario()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }

        public int UsuarioId { get; set; }
        public string Chave { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? CpfRg { get; set; }
        public string? ImagemPerfil { get; set; }
        public bool Ativo { get; set; }
        private DateTime _dataCadastro;
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }
        public Enums.PerfilUsuario Perfil { get; set; }
    }
}