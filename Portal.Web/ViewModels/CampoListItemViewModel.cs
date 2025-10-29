namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class CampoListItemViewModel
    {
        public int CampoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Responsavel { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public int FormulariosVinculados { get; set; }
    }
}
