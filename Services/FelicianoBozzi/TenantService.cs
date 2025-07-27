using System.ComponentModel.DataAnnotations;
using apiBozzi.Context;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class TenantService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<Tenant> AddTenant(NewTenant dto)
    {
        await ValidateTenant(dto);

        var newTenant = new Tenant(dto)
        {
            ResponsibleTenant = dto.ResponsibleTenantId.HasValue()
                ? await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.ResponsibleTenantId)
                : null
        };

        var res = await Context.Tenants.AddAsync(newTenant);
        await Context.SaveChangesAsync();

        return res.Entity;
    }

    public async Task<PagedResult<Tenant>> ListTenants(TenantFilter filter)
    {
        var totalItens = await Context.Tenants.CountAsync();

        var tenants = await Context.Tenants
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var result = new PagedResult<Tenant>
        {
            Items = tenants,
            TotalItems = totalItens,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalItens / filter.PageSize)
        };

        return result;
    }

    #endregion

    private async Task ValidateTenant(NewTenant dto)
    {
        if (!dto.Cpf.IsValidCpf())
            throw new ValidationException("O CPF informado não é válido.");

        if (await Context.Tenants.AnyAsync(x => x.Cpf == dto.Cpf))
            throw new ValidationException("O CPF informado já está sendo utilizado.");

        if (!dto.Email.IsValidEmail())
            throw new ValidationException("O Email informado não é valido.");
    }
}