using System;
using System.Collections.Generic;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class FormulariosIndexViewModel
    {
        public FormularioFiltroViewModel Filtro { get; set; } = new();
        public PaginacaoViewModel Paginacao { get; set; } = new();
        public IReadOnlyCollection<FormularioListItemViewModel> Registros { get; set; } = Array.Empty<FormularioListItemViewModel>();
    }
}
