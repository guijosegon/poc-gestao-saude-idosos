using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class CamposIndexViewModel
    {
        public CampoFiltroViewModel Filtro { get; set; } = new();
        public PaginacaoViewModel Paginacao { get; set; } = new();
        public IReadOnlyCollection<CampoListItemViewModel> Registros { get; set; } = Array.Empty<CampoListItemViewModel>();
        public IEnumerable<SelectListItem> TiposCampo { get; set; } = Array.Empty<SelectListItem>();
    }
}
