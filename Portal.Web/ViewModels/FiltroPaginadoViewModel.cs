namespace GestaoSaudeIdosos.Web.ViewModels
{
    public abstract class FiltroPaginadoViewModel
    {
        private const int ItensPorPaginaPadrao = 10;
        private const int MaximoItensPorPagina = 50;
        private int _pagina = 1;
        private int _itensPorPagina = ItensPorPaginaPadrao;

        public int Pagina
        {
            get => _pagina;
            set => _pagina = value <= 0 ? 1 : value;
        }

        public int ItensPorPagina
        {
            get => _itensPorPagina;
            set
            {
                if (value <= 0)
                {
                    _itensPorPagina = ItensPorPaginaPadrao;
                    return;
                }

                _itensPorPagina = value > MaximoItensPorPagina ? MaximoItensPorPagina : value;
            }
        }
    }
}
