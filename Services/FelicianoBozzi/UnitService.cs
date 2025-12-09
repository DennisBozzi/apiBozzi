using apiBozzi.Exceptions;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Services.Firebase;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class UnitService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    private bool IsAdmin => UserProvider.IsAdmin;

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
            units = await query.ToListAsync();

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

    // public async Task<PagedResult<ApartmentResponse>> ListAvailableApartments(ApartmentFilter filter)
    // {
    //     if (IsAdmin)
    //     {
    //         var apartments = Context.Apartments.AsQueryable();
    //
    //         if (filter.Number != null)
    //             apartments = apartments.Where(x => x.Number.ToLower().Contains(filter.Number.ToLower()));
    //
    //         var items = await apartments
    //             .Skip((filter.Page - 1) * filter.PageSize)
    //             .Take(filter.PageSize)
    //             .OrderBy(x => x.CreatedAt)
    //             .Select(x => new ApartmentResponse(x))
    //             .ToListAsync();
    //
    //         var totalItems = items.Count;
    //
    //         var res = new PagedResult<ApartmentResponse>
    //         {
    //             Items = items,
    //             TotalItems = totalItems,
    //             CurrentPage = filter.Page,
    //             PageSize = filter.PageSize,
    //             TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
    //         };
    //
    //         return res;
    //     }
    //     else
    //     {
    //         var apartments = Context.ApartmentsDemo.AsQueryable();
    //
    //         if (filter.Number != null)
    //             apartments = apartments.Where(x => x.Number.ToLower().Contains(filter.Number.ToLower()));
    //
    //         var items = await apartments
    //             .Skip((filter.Page - 1) * filter.PageSize)
    //             .Take(filter.PageSize)
    //             .OrderBy(x => x.CreatedAt)
    //             .Select(x => new ApartmentResponse(x))
    //             .ToListAsync();
    //
    //         var totalItems = items.Count;
    //
    //         var res = new PagedResult<ApartmentResponse>
    //         {
    //             Items = items,
    //             TotalItems = totalItems,
    //             CurrentPage = filter.Page,
    //             PageSize = filter.PageSize,
    //             TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
    //         };
    //
    //         return res;
    //     }
    // }

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
        await Context.SaveChangesAsync();

        return new UnitResponse(res.Entity);
    }

    // public async Task<ApartmentResponse> MakeResponsible(int idApartment, int idTenant)
    // {
    //     if (IsAdmin)
    //     {
    //         var ap = await Context.Apartments.FirstOrDefaultAsync(x => x.Id == idApartment);
    //         var ten = await Context.Tenants.FirstOrDefaultAsync(x => x.Id == idTenant);
    //
    //         _ValidateMakeResponsible(ap, ten);
    //
    //         await Context.SaveChangesAsync();
    //
    //         return new ApartmentResponse(ap);
    //     }
    //     else
    //     {
    //         var ap = await Context.ApartmentsDemo.FirstOrDefaultAsync(x => x.Id == idApartment);
    //         var ten = await Context.TenantsDemo.FirstOrDefaultAsync(x => x.Id == idTenant);
    //
    //         _ValidateMakeResponsible(ap, ten);
    //
    //         await Context.SaveChangesAsync();
    //
    //         return new ApartmentResponse(ap);
    //     }
    // }

    // public async Task<ApartmentResponse> RemoveResponsible(int idApartment)
    // {
    //     if (IsAdmin)
    //     {
    //         var ap = await Context.Apartments.FirstOrDefaultAsync(x => x.Id == idApartment);
    //
    //         _ValidateRemoveResponsible(ap);
    //
    //         await Context.SaveChangesAsync();
    //
    //         return new ApartmentResponse(ap);
    //     }
    //     else
    //     {
    //         var ap = await Context.ApartmentsDemo.FirstOrDefaultAsync(x => x.Id == idApartment);
    //
    //         _ValidateRemoveResponsible(ap);
    //
    //         await Context.SaveChangesAsync();
    //
    //         return new ApartmentResponse(ap);
    //     }
    // }

    // public async Task<ApartmentResponse> GetApartmentById(int idApartment)
    // {
    //     if (IsAdmin)
    //     {
    //         var apartment = await Context.Apartments.FirstOrDefaultAsync(x => x.Id == idApartment);
    //
    //         if (apartment == null)
    //             throw new ValidationException("O apartamento não foi encontrado.");
    //
    //         var res = new ApartmentResponse(apartment);
    //
    //         return res;
    //     }
    //     else
    //     {
    //         var apartment = await Context.ApartmentsDemo.FirstOrDefaultAsync(x => x.Id == idApartment);
    //
    //         if (apartment == null)
    //             throw new ValidationException("O apartamento não foi encontrado.");
    //
    //         var res = new ApartmentResponse(apartment);
    //         
    //         return res;
    //     }
    // }

    // public async Task<ApartmentResponse> GetApartmentByNumber(string numberApartment)
    // {
    //     if (IsAdmin)
    //     {
    //         var apartment = await Context.Apartments.FirstOrDefaultAsync(x => x.Number.ToUpper().Equals(numberApartment.ToUpper()));
    //
    //         if (apartment == null)
    //             throw new ValidationException("O apartamento não foi encontrado.");
    //
    //         var res = new ApartmentResponse(apartment);
    //
    //         return res;
    //     }
    //     else
    //     {
    //         var apartment = await Context.ApartmentsDemo.FirstOrDefaultAsync(x => x.Number.ToUpper().Equals(numberApartment.ToUpper()));
    //
    //         if (apartment == null)
    //             throw new ValidationException("O apartamento não foi encontrado.");
    //
    //         var res = new ApartmentResponse(apartment);
    //
    //         return res;
    //     }
    // }
    //

    #endregion


    #region Private

    private void _ExistUnit(string number)
    {
        var existeAp = Context.Units.Any(x => x.Number.ToLower().Equals(number.ToLower()));
        if (existeAp)
            throw new ValidationException("Já existe um apartamento com esse número.");
    }

    // private void _ExistApartmentDemo(string numero)
    // {
    //     var existeAp = Context.ApartmentsDemo.Any(x => x.Number.ToLower().Equals(numero.ToLower()));
    //     if (existeAp)
    //         throw new ValidationException("Já existe um apartamento demo com esse número.");
    // }
    //
    // private void _ValidateMakeResponsible(Apartment? ap, Tenant? ten)
    // {
    //     if (ten.IsEmpty())
    //         throw new ValidationException("O inquilino não foi encontrado.");
    //
    //     if (ap.IsEmpty())
    //         throw new ValidationException("O apartamento não foi encontrado.");
    // }
    //
    // private void _ValidateMakeResponsible(ApartmentDemo? ap, TenantDemo? ten)
    // {
    //     if (ten.IsEmpty())
    //         throw new ValidationException("O inquilino não foi encontrado.");
    //
    //     if (ap.IsEmpty())
    //         throw new ValidationException("O apartamento não foi encontrado.");
    // }
    //
    // private void _ValidateRemoveResponsible(Apartment? ap)
    // {
    //     if (ap.IsEmpty())
    //         throw new ValidationException("O apartamento não foi encontrado.");
    // }
    //
    // private void _ValidateRemoveResponsible(ApartmentDemo? ap)
    // {
    //     if (ap.IsEmpty())
    //         throw new ValidationException("O apartamento não foi encontrado.");
    // }
    //

    #endregion
}