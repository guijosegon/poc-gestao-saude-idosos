namespace GestaoSaudeIdosos.Domain.Entities
{
    public class FormularioCampo
    {
        public FormularioCampo() 
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
        }

        public int FormularioCampoId { get; set; }
        public string Chave { get; set; }
        public int Ordem { get; set; }
        public bool Obrigatorio { get; set; }
        public DateTime DataCadastro { get; set; }

        public int FormularioId { get; set; }
        public Formulario Formulario { get; set; }

        public int CampoId { get; set; }
        public Campo Campo { get; set; }
    }
}
