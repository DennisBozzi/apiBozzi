using System.ComponentModel.DataAnnotations;
using apiBozzi.Context;
using apiBozzi.Models;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class TenantService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
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

        var res = await Context.Tenants.AddAsync(newTenant);
        await Context.SaveChangesAsync();

        return new TenantResponse(res.Entity);
    }

    public async Task<PagedResult<TenantResponse>> ListTenants(TenantFilter filter)
    {
        var totalItems = await Context.Tenants.CountAsync();

        var tenants = await Context.Tenants
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TenantResponse(x))
            .ToListAsync();

        var result = new PagedResult<TenantResponse>
        {
            Items = tenants,
            TotalItems = totalItems,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize)
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

        var responsible = await Context.Tenants
            .Include(x => x.Responsible)
            .FirstOrDefaultAsync(x => x.Id == dto.ResponsibleTenantId);

        if (responsible.HasValue() && responsible.Responsible.HasValue())
            throw new ValidationException("O responsável informado é inválido.");
    }
}