using GestaoSaudeIdosos.Domain.Common.Helpers;

namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class UsuarioFiltroViewModel : FiltroPaginadoViewModel
    {
        public string? Busca { get; set; }
        public Enums.PerfilUsuario? Perfil { get; set; }
        public bool? Ativo { get; set; }
    }
}
