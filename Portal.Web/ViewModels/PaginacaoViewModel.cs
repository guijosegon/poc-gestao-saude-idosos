namespace GestaoSaudeIdosos.Web.ViewModels
{
    public class PaginacaoViewModel
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int ItensPorPagina { get; set; }

        public bool PossuiPaginas => TotalPaginas > 1;
        public int PrimeiraPagina => TotalPaginas == 0 ? 0 : 1;
        public int UltimaPagina => TotalPaginas;
        public bool PossuiPaginaAnterior => PaginaAtual > PrimeiraPagina;
        public bool PossuiProximaPagina => PaginaAtual < TotalPaginas;
        public int PaginaAnterior => Math.Max(PaginaAtual - 1, PrimeiraPagina);
        public int ProximaPagina => Math.Min(PaginaAtual + 1, TotalPaginas);
    }
}
