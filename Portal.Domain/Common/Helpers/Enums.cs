using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Domain.Common.Helpers
{
    public class Enums
    {
        public enum PerfilUsuario
        {
            [Display(Name = "Comum")]
            Comum = 0,

            [Display(Name = "Administrador")] 
            Administrador = 1
        }

        public enum TipoCampo
        {
            [Display(Name = "Texto")]
            Texto = 1,
            [Display(Name = "Número")]
            Numero = 2,
            [Display(Name = "Data")]
            Data = 3,
            [Display(Name = "Data e hora")]
            DataHora = 4,
            [Display(Name = "Seleção")]
            Selecao = 5,
            [Display(Name = "Checkbox")]
            Checkbox = 6
        }
    }
}