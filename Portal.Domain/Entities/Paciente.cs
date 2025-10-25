using GestaoSaudeIdosos.Domain.Extensions;
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
        private DateTime _dataNascimento;
        private DateTime _dataCadastro;

        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => _dataNascimento = value.EnsureUtc();
        }

        public int Idade { get; set; }
        public bool Ativo { get; set; }

        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }

        public int? ResponsavelId { get; set; }
        public virtual Usuario? Responsavel { get; set; }
        public int? UsuarioCadastroId { get; set; }
        public virtual Usuario? UsuarioCadastro { get; set; }
    }
}
