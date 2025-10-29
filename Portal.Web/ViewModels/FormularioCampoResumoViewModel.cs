namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioCampoResumoViewModel
    {
        public string NomeCampo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Obrigatorio { get; set; }
        public int Ordem { get; set; }
    }
}
