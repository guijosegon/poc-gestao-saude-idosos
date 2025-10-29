using GestaoSaudeIdosos.Domain.Extensions;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class FormularioResultado
    {
        public FormularioResultado()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
            Valores = new List<FormularioResultadoValor>();
        }

        public int FormularioResultadoId { get; set; }
        public string Chave { get; set; }

        private DateTime _dataPreenchimento;
        public DateTime DataPreenchimento
        {
            get => _dataPreenchimento;
            set => _dataPreenchimento = value.EnsureUtc();
        }

        public bool Ativo { get; set; }

        private DateTime _dataCadastro;
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        public int FormularioId { get; set; }
        public Formulario Formulario { get; set; } = null!;

        public int? UsuarioAplicacaoId { get; set; }
        public Usuario? UsuarioAplicacao { get; set; }

        public ICollection<FormularioResultadoValor> Valores { get; set; }
    }
}
