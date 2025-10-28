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

        public enum CondicaoCronicaPaciente
        {
            [Display(Name = "Diabetes mellitus (DM)")]
            DiabetesMellitus = 1,
            [Display(Name = "Hipertensão arterial sistêmica (HAS)")]
            HipertensaoArterial = 2,
            [Display(Name = "Doença pulmonar obstrutiva crônica (DPOC)")]
            DoencaPulmonarObstrutivaCronica = 3,
            [Display(Name = "Demência")]
            Demencia = 4,
            [Display(Name = "Doença cardiovascular")]
            DoencaCardiovascular = 5,
            [Display(Name = "Doença renal crônica")]
            DoencaRenalCronica = 6,
            [Display(Name = "Outra condição crônica")]
            Outra = 99
        }

        public enum HistoricoCirurgicoPaciente
        {
            [Display(Name = "Sem histórico cirúrgico")]
            Nenhum = 1,
            [Display(Name = "Cirurgia cardíaca")]
            CirurgiaCardiaca = 2,
            [Display(Name = "Cirurgia ortopédica")]
            CirurgiaOrtopedica = 3,
            [Display(Name = "Cirurgia neurológica")]
            CirurgiaNeurologica = 4,
            [Display(Name = "Cirurgia gastrointestinal")]
            CirurgiaGastrointestinal = 5,
            [Display(Name = "Cirurgia ocular")]
            CirurgiaOcular = 6,
            [Display(Name = "Outro histórico cirúrgico")]
            Outro = 99
        }

        public enum RiscoQuedaPaciente
        {
            [Display(Name = "Sem risco identificado")]
            SemRisco = 1,
            [Display(Name = "Risco baixo")]
            Baixo = 2,
            [Display(Name = "Risco moderado")]
            Moderado = 3,
            [Display(Name = "Risco alto")]
            Alto = 4,
            [Display(Name = "Quedas recorrentes")]
            QuedasRecorrentes = 5
        }

        public enum MobilidadePaciente
        {
            [Display(Name = "Independente")]
            Independente = 1,
            [Display(Name = "Bengala")]
            Bengala = 2,
            [Display(Name = "Andador")]
            Andador = 3,
            [Display(Name = "Cadeira de rodas")]
            CadeiraDeRodas = 4,
            [Display(Name = "Acamado")]
            Acamado = 5,
            [Display(Name = "Prótese ou órtese")]
            ProteseOuOrtese = 6,
            [Display(Name = "Outro auxílio")]
            Outro = 99
        }

        public enum DietaRestricaoPaciente
        {
            [Display(Name = "Sem restrições")]
            SemRestricoes = 1,
            [Display(Name = "Dieta para diabetes")]
            Diabetes = 2,
            [Display(Name = "Dieta hipossódica")]
            Hipossodica = 3,
            [Display(Name = "Sem glúten")]
            SemGluten = 4,
            [Display(Name = "Sem lactose")]
            SemLactose = 5,
            [Display(Name = "Dieta hiperproteica")]
            Hiperproteica = 6,
            [Display(Name = "Outra restrição alimentar")]
            Outra = 99
        }

        public enum SexoPaciente
        {
            [Display(Name = "Não informado")]
            NaoInformado = 0,

            [Display(Name = "Feminino")]
            Feminino = 1,

            [Display(Name = "Masculino")]
            Masculino = 2,

            [Display(Name = "Outro")]
            Outro = 99
        }
    }
}