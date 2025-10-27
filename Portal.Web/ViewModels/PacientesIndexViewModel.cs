using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PacientesIndexViewModel
    {
        public PacienteFiltroViewModel Filtro { get; set; } = new();
        public PaginacaoViewModel Paginacao { get; set; } = new();
        public IReadOnlyCollection<PacienteListItemViewModel> Registros { get; set; } = Array.Empty<PacienteListItemViewModel>();
        public IEnumerable<SelectListItem> Responsaveis { get; set; } = Array.Empty<SelectListItem>();
    }
}
