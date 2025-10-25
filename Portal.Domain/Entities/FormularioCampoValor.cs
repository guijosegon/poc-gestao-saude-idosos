using GestaoSaudeIdosos.Domain.Extensions;
using System;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class FormularioCampoValor
    {
        public FormularioCampoValor()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
        } 

        public int FormularioCampoValorId { get; set; }
        public string Chave { get; set; }
        public string Valor { get; set; }
        private DateTime _dataCadastro;
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }
        private DateTime? _dataPreenchimento;
        public DateTime? DataPreenchimento
        {
            get => _dataPreenchimento;
            set => _dataPreenchimento = value.EnsureUtc();
        }

        public int FormularioCampoId { get; set; }
        public FormularioCampo FormularioCampo { get; set; }

        public int FormularioId { get; set; }
        public Formulario Formulario { get; set; }
    }
}