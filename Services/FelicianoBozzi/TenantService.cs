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
    #region Main

    public async Task<TenantResponse> AddTenant(NewTenant dto)
    {
        await ValidateTenant(dto);
        var newTenant = new Tenant(dto);

        var res = await Context.Tenants.AddAsync(newTenant);
        return new TenantResponse(res.Entity);
    }

    public async Task<PagedResult<TenantResponse>> ListTenants(TenantFilter filter)
    {
        var tenants = Context.Tenants.AsQueryable();

        if (filter.NameCpf != null)
            tenants = tenants.Where(x =>
                x.FirstName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                x.LastName.ToLower().StartsWith(filter.NameCpf.ToLower()) ||
                x.Cpf.ToLower().StartsWith(filter.NameCpf.Replace(".", "").Replace("-", "").ToLower()));

        var totalItems = await tenants
            .CountAsync();

        var items = tenants.ToList()
            .Select(x => new TenantResponse(x))
            .OrderBy(x => x.FirstName);

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

    public async Task<TenantResponse?> GetTenantById(int id)
    {
        var tenant = await Context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
        return tenant == null ? null : new TenantResponse(tenant);
    }

    #endregion

    #region Private

    private async Task ValidateTenant(NewTenant dto)
    {
        if (!dto.Cpf.IsValidCpf())
            throw new ValidationException("The document provided is not valid.");

        if (await Context.Tenants.AnyAsync(x => x.Cpf == dto.Cpf))
            throw new ValidationException("The document provided is already in use.");

        if (!dto.Email.IsValidEmail())
            throw new ValidationException("The email address provided is not valid.");
    }

    #endregion
}