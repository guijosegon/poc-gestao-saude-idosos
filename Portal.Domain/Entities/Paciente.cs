using System;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Paciente
    {
        public Paciente()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }

        public int PacienteId { get; set; }
        public string Chave { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Idade { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        public int? ResponsavelId { get; set; }
        public virtual Usuario? Responsavel { get; set; }
        public int? UsuarioCadastroId { get; set; }
        public virtual Usuario? UsuarioCadastro { get; set; }
    }
}
