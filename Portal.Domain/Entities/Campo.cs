using GestaoSaudeIdosos.Domain.Common.Helpers;
using GestaoSaudeIdosos.Domain.Extensions;

namespace GestaoSaudeIdosos.Domain.Entities
{
    public class Campo
    {
        public Campo()
        {
            Chave = Guid.NewGuid().ToString();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
            Formularios = new List<FormularioCampo>();
            ResultadoValores = new List<FormularioResultadoValor>();
        }

        public int CampoId { get; set; }
        public string Chave { get; set; }
        public string Descricao { get; set; }
        public Enums.TipoCampo Tipo { get; set; }
        public List<string> Opcoes { get; set; }
        public string TextoAjuda { get; set; }
        public bool Ativo { get; set; }

        private DateTime _dataCadastro;
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => _dataCadastro = value.EnsureUtc();
        }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<FormularioCampo> Formularios { get; set; }
        public ICollection<FormularioResultadoValor> ResultadoValores { get; set; }
    }
}
