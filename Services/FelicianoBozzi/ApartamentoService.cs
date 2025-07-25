using apiBozzi.Context;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class ApartamentoService
{
    private readonly AppDbContext _context;

    public ApartamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<PagedResult<Apartamento>>> ListerApartamentos(ApartamentoFiltro apartamentoFiltro)
    {
        var res = new ServiceResponse<PagedResult<Apartamento>>();

        try
        {
            var totalItens = await _context.Apartamentos.CountAsync();

            var apartamentos = await _context.Apartamentos
                .Skip((apartamentoFiltro.Pagina - 1) * apartamentoFiltro.TamanhoPagina)
                .Take(apartamentoFiltro.TamanhoPagina)
                .OrderBy(x => x.Numero)
                .ToListAsync();

            var resultado = new PagedResult<Apartamento>
            {
                Items = apartamentos,
                TotalItens = totalItens,
                PaginaAtual = apartamentoFiltro.Pagina,
                TamanhoPagina = apartamentoFiltro.TamanhoPagina,
                TotalPaginas = (int)Math.Ceiling((double)totalItens / apartamentoFiltro.TamanhoPagina)
            };

            res.Objeto = resultado;
            res.Mensagem = "Apartamentos obtidos com sucesso!";
        }
        catch (Exception e)
        {
            res.Mensagem = e.Message;
            res.Successo = false;
        }

        return res;
    }

    public async Task<ServiceResponse<Apartamento>> CriarApartamento(string numero, int andar, decimal valor)
    {
        var res = new ServiceResponse<Apartamento>();

        try
        {
            var existeAp = _context.Apartamentos.Any(x => x.Numero.ToLower().Equals(numero.ToLower()));
            if (existeAp)
                throw new Exception("Já existe um apartamento com esse número.");

            var newAp = new Apartamento
            {
                Numero = numero.ToUpper(),
                Andar = (AndarEnum)andar,
                ValorAluguel = valor
            };

            var ap = await _context.Apartamentos.AddAsync(newAp);
            await _context.SaveChangesAsync();

            res.Objeto = ap.Entity;
            res.Mensagem = "Apartamento cadastrado com sucesso!";
        }
        catch (Exception e)
        {
            res.Mensagem = e.Message;
            res.Successo = false;
        }

        return res;
    }
}