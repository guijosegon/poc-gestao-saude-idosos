namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormularioFiltroViewModel : FiltroPaginadoViewModel
    {
        public string? Busca { get; set; }
        public bool? Ativo { get; set; }
    }
}
