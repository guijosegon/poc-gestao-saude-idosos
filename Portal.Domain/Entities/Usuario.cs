using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Usuario
    {
        public Usuario()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.Now;
            Ativo = true;
        }

        public int UsuarioId { get; set; }
        public string Chave { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public Enums.PerfilUsuario Perfil { get; set; }
    }
}