using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Campo
    {
        public Campo()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTimeOffset.Now;
            Ativo = true;
        }

        public int CampoId { get; set; }
        public string Chave { get; set; }
        public string Descricao { get; set; }
        public Enums.TipoCampo Tipo { get; set; }
        public List<string> Opcoes { get; set; }
        public string TextoAjuda { get; set; }
        public bool Ativo { get; set; }
        public DateTimeOffset DataCadastro { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<FormularioCampoValor> FormularioCampos { get; set; }
    }
}