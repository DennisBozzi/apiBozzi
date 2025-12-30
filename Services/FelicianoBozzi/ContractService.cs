using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class ContractService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<ContractResponse> NewContract(NewContract dto)
    {
        dto.Tenant = await Context.Tenants.FirstOrDefaultAsync(x => x.Id == dto.TenantId);
        dto.Unit = await Context.Units.FirstOrDefaultAsync(x => x.Id == dto.UnitId);

        ValidateContract(dto);

        var contract = new Contract(dto);
        //TODO: Criar o Contrato (File) antes de salvar.

        Context.Contracts.Add(contract);

        return new ContractResponse();
    }

    #endregion

    #region Private

    private void ValidateContract(NewContract dto)
    {
        if (dto.Tenant is null)
            throw new ValidationException("No tenant found with this identifier.");

        if (dto.Unit is null)
            throw new ValidationException("No unit found with this identifier.");
    }

    #endregion
}