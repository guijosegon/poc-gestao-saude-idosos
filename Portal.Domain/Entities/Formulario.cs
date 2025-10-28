using GestaoSaudeIdosos.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Formulario
    {
        public Formulario()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
            Campos = new List<FormularioCampo>();
            Pacientes = new List<Paciente>();
            Resultados = new List<FormularioResultado>();
        }

        public int FormularioId { get; set; }
        public string Chave { get; set; }
        public string Descricao { get; set; }

        private DateTime _dataCadastro;
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }

        public bool Ativo { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<FormularioCampo> Campos { get; set; }
        public ICollection<Paciente> Pacientes { get; set; }
        public ICollection<FormularioResultado> Resultados { get; set; }
    }
}
