using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class CampoFiltroViewModel : FiltroPaginadoViewModel
    {
        public string? Busca { get; set; }
        public Enums.TipoCampo? Tipo { get; set; }
        public bool? Ativo { get; set; }
    }
}
