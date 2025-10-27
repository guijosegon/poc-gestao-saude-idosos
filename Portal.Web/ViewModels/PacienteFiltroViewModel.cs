namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacienteFiltroViewModel : FiltroPaginadoViewModel
    {
        public string? Busca { get; set; }
        public int? ResponsavelId { get; set; }
    }
}
