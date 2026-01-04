using System.Globalization;
using apiBozzi.Exceptions;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Services.Firebase;
using apiBozzi.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class UnitService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<PagedResult<UnitResponse>> ListUnits(UnitFilter unitFilter)
    {
        var totalItems = await Context.Units.CountAsync();

        var units = new List<UnitResponse>();

        var query = Context.Units
            .Skip((unitFilter.Page - 1) * unitFilter.PageSize)
            .Take(unitFilter.PageSize)
            .OrderBy(x => x.Number)
            .Select(x => new UnitResponse(x));

        if (query.Any())
        {
            units = await query.ToListAsync();
            units = await ContractService.FillContracts(units);
        }

        var res = new PagedResult<UnitResponse>
        {
            Items = units,
            TotalItems = totalItems,
            CurrentPage = unitFilter.Page,
            PageSize = unitFilter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / unitFilter.PageSize)
        };

        return res;
    }

    public async Task<UnitResponse?> GetUnitById(int id)
    {
        var unit = await Context.Units.FirstOrDefaultAsync(x => x.Id == id);
        return unit == null ? null : new UnitResponse(unit);
    }

    public async Task<UnitResponse> AddUnit(NewUnit value)
    {
        _ExistUnit(value.Number);

        var newUni = new Unit
        {
            Number = value.Number.ToUpper(),
            Floor = (Floor)value.Floor,
            Type = (UnitType)value.Type,
        };

        var res = await Context.Units.AddAsync(newUni);

        return new UnitResponse(res.Entity);
    }

    #endregion

    #region Private

    private void _ExistUnit(string number)
    {
        var existeAp = Context.Units.Any(x => x.Number.ToLower().Equals(number.ToLower()));
        if (existeAp)
            throw new ValidationException("A unit with the specified number already exists.");
    }

    #endregion
}