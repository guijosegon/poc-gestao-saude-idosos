using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Domain.Common.Helpers
{
    public class Enums
    {
        public enum PerfilUsuario
        {
            [Display(Name = "Administrador")]
            Administrador = 1,

            [Display(Name = "Comum")]
            Comum = 2
        }
    }
}