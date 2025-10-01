namespace GestaoSaudeIdosos.Domain.Entities
{
    public class FormularioResultado
    {
        public FormularioResultado()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.Now;
            Ativo = true;
        }

        public int FormularioResultadoId { get; set; }
        public string Chave { get; set; }
        public DateTime DataPreenchimento { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        public int FormularioId { get; set; }
        public Formulario Formulario { get; set; }


        //public ICollection<FormularioCampoValor> Campos { get; set; }
    }
}
