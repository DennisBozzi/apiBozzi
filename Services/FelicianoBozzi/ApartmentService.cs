using apiBozzi.Exceptions;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class ApartmentService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<PagedResult<ApartmentResponse>> ListApartments(ApartmentFilter apartmentFilter)
    {
        var totalItems = await Context.Apartments.CountAsync();

        var apartments = await Context.Apartments
            .Skip((apartmentFilter.Page - 1) * apartmentFilter.PageSize)
            .Take(apartmentFilter.PageSize)
            .OrderBy(x => x.Number)
            .Include(x => x.Responsible)
            .Select(x => new ApartmentResponse(x))
            .ToListAsync();

        var res = new PagedResult<ApartmentResponse>
        {
            Items = apartments,
            TotalItems = totalItems,
            CurrentPage = apartmentFilter.Page,
            PageSize = apartmentFilter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / apartmentFilter.PageSize)
        };

        return res;
    }

    public async Task<ApartmentResponse> AddApartment(NewApartment value)
    {
        ValidateApartment(value.Number);

        var newAp = new Apartment
        {
            Number = value.Number.ToUpper(),
            Floor = (FloorEnum)value.Floor,
            Type = (ApartmentTypeEnum)value.Type,
            Rent = value.RentValue,
        };

        var res = await Context.Apartments.AddAsync(newAp);
        await Context.SaveChangesAsync();

        return new ApartmentResponse(res.Entity);
    }

    public async Task<ApartmentResponse> MakeResponsible(int idApartment, int idTenant)
    {
        var ap = await Context.Apartments.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idApartment);
        var ten = await Context.Tenants.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idTenant);

        ValidateResponsible(ap, ten);

        ap.Responsible = ten;
        Context.SaveChangesAsync();

        return new ApartmentResponse(ap);
    }

    public async Task<ApartmentResponse> GetApartmentById(int idApartment)
    {
        var apartment = await Context.Apartments
            .Include(x => x.Responsible)
            .FirstOrDefaultAsync(x => x.Id == idApartment);

        if (apartment == null)
            throw new ValidationException("O apartamento não foi encontrado.");

        var res = new ApartmentResponse(apartment);

        if (apartment.Responsible == null)
            return res;

        var residents = Context.Tenants
            .Where(x => x.Responsible == apartment.Responsible || x == apartment.Responsible)
            .ToList();

        res.WithResidents(residents);

        return res;
    }

    public async Task<ApartmentResponse> GetApartmentByNumber(string numberApartment)
    {
        var apartment = await Context.Apartments
            .Include(x => x.Responsible)
            .FirstOrDefaultAsync(x => x.Number.Equals(numberApartment, StringComparison.CurrentCultureIgnoreCase));

        if (apartment == null)
            throw new ValidationException("O apartamento não foi encontrado.");

        var res = new ApartmentResponse(apartment);

        if (apartment.Responsible == null)
            return res;

        var residents = Context.Tenants
            .Where(x => x.Responsible == apartment.Responsible || x == apartment.Responsible)
            .ToList();

        res.WithResidents(residents);

        return res;
    }

    #endregion

    private void ValidateApartment(string numero)
    {
        var existeAp = Context.Apartments.Any(x => x.Number.ToLower().Equals(numero.ToLower()));
        if (existeAp)
            throw new ValidationException("Já existe um apartamento com esse número.");
    }

    private void ValidateResponsible(Apartment? ap, Tenant? ten)
    {
        if (ten.IsEmpty())
            throw new ValidationException("O inquilino não foi encontrado.");

        if (ap.IsEmpty())
            throw new ValidationException("O apartamento não foi encontrado.");

        if (ap.Responsible.HasValue())
            throw new ValidationException("O apartamento já possui um responsável.");

        if (ten.Responsible.HasValue())
            throw new ValidationException("O inquilino não é válido.");
    }
}