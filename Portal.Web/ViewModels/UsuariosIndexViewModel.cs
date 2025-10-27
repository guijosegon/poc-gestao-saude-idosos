using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuariosIndexViewModel
    {
        public UsuarioFiltroViewModel Filtro { get; set; } = new();
        public PaginacaoViewModel Paginacao { get; set; } = new();
        public IReadOnlyCollection<UsuarioListItemViewModel> Registros { get; set; } = Array.Empty<UsuarioListItemViewModel>();
        public IEnumerable<SelectListItem> Perfis { get; set; } = Array.Empty<SelectListItem>();
    }
}
