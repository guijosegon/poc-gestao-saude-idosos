using GestaoSaudeIdosos.Domain.Extensions;
using System;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class FormularioResultadoValor
    {
        public FormularioResultadoValor()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
        }

        public int FormularioResultadoValorId { get; set; }
        public string Chave { get; set; } = string.Empty;
        public string? Valor { get; set; }

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

        public int FormularioResultadoId { get; set; }
        public FormularioResultado FormularioResultado { get; set; } = null!;

        public int FormularioCampoId { get; set; }
        public FormularioCampo FormularioCampo { get; set; } = null!;

        public int CampoId { get; set; }
        public Campo Campo { get; set; } = null!;
    }
}
