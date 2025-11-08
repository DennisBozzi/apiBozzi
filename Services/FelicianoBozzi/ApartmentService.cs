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

public class ApartmentService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    private readonly FirebaseUserProvider _userProvider = serviceProvider.GetRequiredService<FirebaseUserProvider>();
    private bool IsAdmin => _userProvider.IsAdmin;

    #region Main

    public async Task<PagedResult<ApartmentResponse>> ListApartments(ApartmentFilter apartmentFilter)
    {
        if (IsAdmin)
        {
            var totalItems = await Context.Apartments.CountAsync();

            var apartments = new List<ApartmentResponse>();

            var query = Context.Apartments
                .Skip((apartmentFilter.Page - 1) * apartmentFilter.PageSize)
                .Take(apartmentFilter.PageSize)
                .OrderBy(x => x.Number)
                .Include(x => x.Responsible)
                .Select(x => new ApartmentResponse(x));

            if (query.Any())
                apartments = await query.ToListAsync();

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
        else
        {
            var totalItems = await Context.ApartmentsDemo.CountAsync();

            var apartments = new List<ApartmentResponse>();

            var query = Context.ApartmentsDemo
                .Skip((apartmentFilter.Page - 1) * apartmentFilter.PageSize)
                .Take(apartmentFilter.PageSize)
                .OrderBy(x => x.Number)
                .Include(x => x.Responsible)
                .Select(x => new ApartmentResponse(x));

            if (query.Any())
                apartments = await query.ToListAsync();

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
    }

    public async Task<PagedResult<ApartmentResponse>> ListAvailableApartments(ApartmentFilter filter)
    {
        if (IsAdmin)
        {
            var apartments = Context.Apartments
                .Include(x => x.Responsible)
                .Where(x => x.Responsible == null);

            if (filter.Number != null)
                apartments = apartments.Where(x => x.Number.ToLower().Contains(filter.Number.ToLower()));

            var items = await apartments
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new ApartmentResponse(x))
                .ToListAsync();

            var totalItems = items.Count;

            var res = new PagedResult<ApartmentResponse>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
            };

            return res;
        }
        else
        {
            var apartments = Context.ApartmentsDemo
                .Include(x => x.Responsible)
                .Where(x => x.Responsible == null);

            if (filter.Number != null)
                apartments = apartments.Where(x => x.Number.ToLower().Contains(filter.Number.ToLower()));

            var items = await apartments
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new ApartmentResponse(x))
                .ToListAsync();

            var totalItems = items.Count;

            var res = new PagedResult<ApartmentResponse>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
            };

            return res;
        }
    }

    public async Task<ApartmentResponse> AddApartment(NewApartment value)
    {
        if (IsAdmin)
        {
            _ExistApartment(value.Number);

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
        else
        {
            _ExistApartmentDemo(value.Number);

            var newAp = new ApartmentDemo
            {
                Number = value.Number.ToUpper(),
                Floor = (FloorEnum)value.Floor,
                Type = (ApartmentTypeEnum)value.Type,
                Rent = value.RentValue,
            };

            var res = await Context.ApartmentsDemo.AddAsync(newAp);
            await Context.SaveChangesAsync();

            return new ApartmentResponse(res.Entity);
        }
    }

    public async Task<ApartmentResponse> MakeResponsible(int idApartment, int idTenant)
    {
        if (IsAdmin)
        {
            var ap = await Context.Apartments.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idApartment);
            var ten = await Context.Tenants.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idTenant);

            _ValidateMakeResponsible(ap, ten);

            ap!.Responsible = ten;
            await Context.SaveChangesAsync();

            return new ApartmentResponse(ap);
        }
        else
        {
            var ap = await Context.ApartmentsDemo.Include(x => x.Responsible)
                .FirstOrDefaultAsync(x => x.Id == idApartment);
            var ten = await Context.TenantsDemo.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idTenant);

            _ValidateMakeResponsible(ap, ten);

            ap!.Responsible = ten;
            await Context.SaveChangesAsync();

            return new ApartmentResponse(ap);
        }
    }

    public async Task<ApartmentResponse> RemoveResponsible(int idApartment)
    {
        if (IsAdmin)
        {
            var ap = await Context.Apartments.Include(x => x.Responsible).FirstOrDefaultAsync(x => x.Id == idApartment);

            _ValidateRemoveResponsible(ap);

            ap!.Responsible = null;
            await Context.SaveChangesAsync();

            return new ApartmentResponse(ap);
        }
        else
        {
            var ap = await Context.ApartmentsDemo.Include(x => x.Responsible)
                .FirstOrDefaultAsync(x => x.Id == idApartment);

            _ValidateRemoveResponsible(ap);

            ap!.Responsible = null;
            await Context.SaveChangesAsync();

            return new ApartmentResponse(ap);
        }
    }

    public async Task<ApartmentResponse> GetApartmentById(int idApartment)
    {
        if (IsAdmin)
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
        else
        {
            var apartment = await Context.ApartmentsDemo
                .Include(x => x.Responsible)
                .FirstOrDefaultAsync(x => x.Id == idApartment);

            if (apartment == null)
                throw new ValidationException("O apartamento não foi encontrado.");

            var res = new ApartmentResponse(apartment);

            if (apartment.Responsible == null)
                return res;

            var residents = Context.TenantsDemo
                .Where(x => x.Responsible == apartment.Responsible || x == apartment.Responsible)
                .ToList();

            res.Residents = residents.Select(x => new TenantResponse(x, false)).ToList();

            return res;
        }
    }

    public async Task<ApartmentResponse> GetApartmentByNumber(string numberApartment)
    {
        if (IsAdmin)
        {
            var apartment = await Context.Apartments
                .Include(x => x.Responsible)
                .FirstOrDefaultAsync(x => x.Number.ToUpper().Equals(numberApartment.ToUpper()));

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
        else
        {
            var apartment = await Context.ApartmentsDemo
                .Include(x => x.Responsible)
                .FirstOrDefaultAsync(x => x.Number.ToUpper().Equals(numberApartment.ToUpper()));

            if (apartment == null)
                throw new ValidationException("O apartamento não foi encontrado.");

            var res = new ApartmentResponse(apartment);

            if (apartment.Responsible == null)
                return res;

            var residents = Context.TenantsDemo
                .Where(x => x.Responsible == apartment.Responsible || x == apartment.Responsible)
                .ToList();

            res.Residents = residents.Select(x => new TenantResponse(x, false)).ToList();

            return res;
        }
    }

    #endregion

    private void _ExistApartment(string numero)
    {
        var existeAp = Context.Apartments.Any(x => x.Number.ToLower().Equals(numero.ToLower()));
        if (existeAp)
            throw new ValidationException("Já existe um apartamento com esse número.");
    }

    private void _ExistApartmentDemo(string numero)
    {
        var existeAp = Context.ApartmentsDemo.Any(x => x.Number.ToLower().Equals(numero.ToLower()));
        if (existeAp)
            throw new ValidationException("Já existe um apartamento demo com esse número.");
    }

    private void _ValidateMakeResponsible(Apartment? ap, Tenant? ten)
    {
        if (ten.IsEmpty())
            throw new ValidationException("O inquilino não foi encontrado.");

        if (Context.Apartments.Any(x => x.Responsible == ten))
            throw new ValidationException("O inquilino já possui um apartamento.");

        if (ap.IsEmpty())
            throw new ValidationException("O apartamento não foi encontrado.");

        if (ap!.Responsible.HasValue())
            throw new ValidationException("O apartamento já possui um responsável.");

        if (ten.Responsible.HasValue())
            throw new ValidationException("O inquilino não é válido.");
    }

    private void _ValidateMakeResponsible(ApartmentDemo? ap, TenantDemo? ten)
    {
        if (ten.IsEmpty())
            throw new ValidationException("O inquilino não foi encontrado.");

        if (Context.ApartmentsDemo.Any(x => x.Responsible == ten))
            throw new ValidationException("O inquilino já possui um apartamento.");

        if (ap.IsEmpty())
            throw new ValidationException("O apartamento não foi encontrado.");

        if (ap!.Responsible.HasValue())
            throw new ValidationException("O apartamento já possui um responsável.");

        if (ten.Responsible.HasValue())
            throw new ValidationException("O inquilino não é válido.");
    }

    private void _ValidateRemoveResponsible(Apartment? ap)
    {
        if (ap.IsEmpty())
            throw new ValidationException("O apartamento não foi encontrado.");
    }

    private void _ValidateRemoveResponsible(ApartmentDemo? ap)
    {
        if (ap.IsEmpty())
            throw new ValidationException("O apartamento não foi encontrado.");
    }
}