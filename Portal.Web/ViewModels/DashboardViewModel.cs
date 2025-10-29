using System.Collections.Generic;
using System.Linq;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class DashboardViewModel
    {
        public string UsuarioNome { get; set; } = string.Empty;
        public string? UsuarioImagemPerfil { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalPacientes { get; set; }
        public int FormulariosAtivos { get; set; }
        public int AlertasAtivos { get; set; }
        public IEnumerable<FormularioResumoViewModel> FormulariosRecentes { get; set; } = Enumerable.Empty<FormularioResumoViewModel>();
        public IEnumerable<RelatorioPacienteViewModel> Relatorios { get; set; } = Enumerable.Empty<RelatorioPacienteViewModel>();
        public IEnumerable<PacienteListItemViewModel> PacientesRecentes { get; set; } = Enumerable.Empty<PacienteListItemViewModel>();
        public IEnumerable<UsuarioListItemViewModel> UsuariosRecentes { get; set; } = Enumerable.Empty<UsuarioListItemViewModel>();
        public IEnumerable<GraficoVisualizacaoViewModel> Graficos { get; set; } = Enumerable.Empty<GraficoVisualizacaoViewModel>();
    }
}
