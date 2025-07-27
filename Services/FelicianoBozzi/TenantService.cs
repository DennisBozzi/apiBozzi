using apiBozzi.Context;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Utils;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class TenantService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    public async Task<Tenant> AddTenant(NewTenant dto)
    {
        var newTenant = new Tenant(dto)
        {
            ResponsibleTenant = dto.ResponsibleTenantId.HasValue()
                ? await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.ResponsibleTenantId)
                : null
        };

        return newTenant;
    }
}