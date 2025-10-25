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
        public DateTime DataCadastro { get; set; }
        public DateTime? DataPreenchimento { get; set; }

        public int FormularioCampoId { get; set; }
        public FormularioCampo FormularioCampo { get; set; }

        public int FormularioId { get; set; }
        public Formulario Formulario { get; set; }
    }
}