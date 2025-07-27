using apiBozzi.Context;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class ApartamentoService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    public async Task<ServiceResponse<PagedResult<Apartment>>> ListApartments(ApartamentFilter apartamentFilter)
    {
        var res = new ServiceResponse<PagedResult<Apartment>>();

        try
        {
            var totalItens = await Context.Apartments.CountAsync();

            var apartamentos = await Context.Apartments
                .Skip((apartamentFilter.Page - 1) * apartamentFilter.PageSize)
                .Take(apartamentFilter.PageSize)
                .OrderBy(x => x.Number)
                .ToListAsync();

            var resultado = new PagedResult<Apartment>
            {
                Items = apartamentos,
                TotalItems = totalItens,
                CurrentPage = apartamentFilter.Page,
                PageSize = apartamentFilter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItens / apartamentFilter.PageSize)
            };

            res.Object = resultado;
            res.Message = "Apartamentos obtidos com sucesso!";
        }
        catch (Exception e)
        {
            res.Message = e.Message;
            res.Success = false;
        }

        return res;
    }

    public async Task<ServiceResponse<Apartment>> CriarApartamento(string numero, int andar, decimal valor)
    {
        var res = new ServiceResponse<Apartment>();

        try
        {
            var existeAp = Context.Apartments.Any(x => x.Number.ToLower().Equals(numero.ToLower()));
            if (existeAp)
                throw new Exception("Já existe um apartamento com esse número.");

            var newAp = new Apartment
            {
                Number = numero.ToUpper(),
                Floor = (FloorEnum)andar,
                Rent = valor
            };

            var ap = await Context.Apartments.AddAsync(newAp);
            await Context.SaveChangesAsync();

            res.Object = ap.Entity;
            res.Message = "Apartamento cadastrado com sucesso!";
        }
        catch (Exception e)
        {
            res.Message = e.Message;
            res.Success = false;
        }

        return res;
    }
}