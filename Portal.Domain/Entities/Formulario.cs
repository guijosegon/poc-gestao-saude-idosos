namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Formulario
    {
        public Formulario()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.Now;
            Ativo = true;
        }

        public int FormularioId { get; set; }
        public string Chave { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<FormularioCampo> Campos { get; set; }
        public ICollection<Paciente> Pacientes { get; set; }
    }
}
