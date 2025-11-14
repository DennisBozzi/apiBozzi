using System.ComponentModel.DataAnnotations;
using apiBozzi.Context;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Services.Firebase;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class TenantService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    private bool IsAdmin => UserProvider.IsAdmin;

    #region Main

    public async Task<TenantResponse> AddTenant(NewTenant dto)
    {
        await ValidateTenant(dto);
        var newTenant = new Tenant(dto)
        {
            Responsible = dto.ResponsibleTenantId.HasValue()
                ? await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.ResponsibleTenantId)
                : null
        };

        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var res = await Context.Tenants.AddAsync(newTenant);
            await Context.SaveChangesAsync();
            if (dto.ApartmentId > 0)
                await ApartmentService.MakeResponsible(dto.ApartmentId, res.Entity.Id);
            await transaction.CommitAsync();
            return new TenantResponse(res.Entity);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResult<TenantResponse>> ListTenants(TenantFilter filter)
    {
        var apartments = new List<Apartment>();

        var query = Context.Apartments
            .Include(x => x.Responsible)
            .Where(x => x.Responsible != null);

        if (query.Any())
            apartments = await query.ToListAsync();

        var rentedTenantIds = apartments
            .Where(a => a.Responsible != null)
            .Select(a => a.Responsible?.Id)
            .ToList();

        var tenants = Context.Tenants.Include(x => x.Responsible).AsQueryable();

        if (filter.OnlyRented)
            tenants = tenants.Where(x =>
                rentedTenantIds.Contains(x.Id) || rentedTenantIds.Contains(x.Responsible!.Id));

        if (filter.NameCpf != null)
            tenants = tenants.Where(x =>
                x.FirstName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                x.LastName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                x.Cpf.ToLower().StartsWith(filter.NameCpf.Replace(".", "").Replace("-", "").ToLower()));

        var totalItems = await tenants
            .CountAsync();

        var items = tenants.ToList().Select(tenant =>
        {
            var ap = apartments
                .FirstOrDefault(a => a.Responsible == tenant || a.Responsible == tenant.Responsible);

            var res = new TenantResponse(tenant);

            return ap == null ? res : res.WithApartment(ap);
        });

        items = filter.OnlyRented
            ? items.OrderBy(x => x.Apartment?.Number)
            : items.OrderBy(x => x.FirstName);

        var pagedItems = items
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var result = new PagedResult<TenantResponse>
        {
            Items = pagedItems,
            TotalItems = totalItems,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
        };

        return result;
    }

    public async Task<PagedResult<TenantResponse>> ListResponsibleTenants(TenantFilter filter)
    {
        if (IsAdmin)
        {
            var totalItems = await Context.Tenants.CountAsync();

            var tenants = Context.Tenants
                .Where(x => x.Responsible == null)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            if (filter.NameCpf != null)
                tenants = tenants.Where(x =>
                    x.FirstName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                    x.LastName.ToLower().StartsWith(filter.NameCpf.ToLower()));

            var items = await tenants.Include(x => x.Responsible)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new TenantResponse(x, true))
                .ToListAsync();

            var result = new PagedResult<TenantResponse>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
            };

            return result;
        }
        else
        {
            var totalItems = await Context.TenantsDemo.CountAsync();

            var tenants = Context.TenantsDemo
                .Where(x => x.Responsible == null)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            if (filter.NameCpf != null)
                tenants = tenants.Where(x =>
                    x.FirstName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                    x.LastName.ToLower().StartsWith(filter.NameCpf.ToLower()));

            var items = await tenants.Include(x => x.Responsible)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new TenantResponse(x, true))
                .ToListAsync();

            var result = new PagedResult<TenantResponse>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
            };

            return result;
        }
    }

    public async Task<TenantResponse> GetOneTenantAsync(int id)
    {
        var tenant = await Context.Tenants
            .Include(x => x.Responsible)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tenant == null)
            throw new ValidationException("O inquilino não foi encontrado.");

        var res = new TenantResponse(tenant);

        var dependents = await Context.Tenants
            .Where(x => x.Responsible == tenant)
            .ToListAsync();

        var ap = tenant.Responsible != null
            ? await Context.Apartments.FirstOrDefaultAsync(x => x.Responsible == tenant.Responsible)
            : await Context.Apartments.FirstOrDefaultAsync(x => x.Responsible == tenant);

        if (ap != null)
            res.WithApartment(ap);

        res.WithDependents(dependents);

        return res;
    }

    #endregion

    #region Private

    private async Task ValidateTenant(NewTenant dto)
    {
        if (!dto.Cpf.IsValidCpf())
            throw new ValidationException("O CPF informado não é válido.");

        if (await Context.Tenants.AnyAsync(x => x.Cpf == dto.Cpf))
            throw new ValidationException("O CPF informado já está sendo utilizado.");

        if (!dto.Email.IsValidEmail())
            throw new ValidationException("O Email informado não é valido.");

        var responsible = await Context.Tenants
            .Include(x => x.Responsible)
            .FirstOrDefaultAsync(x => x.Id == dto.ResponsibleTenantId);

        if (responsible.HasValue() && responsible.Responsible.HasValue())
            throw new ValidationException("O responsável informado é inválido.");
    }

    private int TotalItems()
    {
        return 0;
    }

    #endregion
}