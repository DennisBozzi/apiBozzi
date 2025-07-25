namespace apiBozzi.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}