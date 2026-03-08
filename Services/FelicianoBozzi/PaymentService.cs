using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class PagamentoService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<ContractResponse?> CreatePaymentsByContract(int id)
    {
        var contract = await Context.Contracts.FindAsync(id);

        if (contract is null) return null;

        var payments = await Context.Payments.Where(x => x.Contract == contract).ToListAsync();

        var range = new List<DateTime>();

        var now = DateTime.Now;

        var months = (contract.ValidUntil.Year - now.Year) * 12 + contract.ValidUntil.Month - now.Month + 1;
        
        return new ContractResponse(contract);
    }

    #endregion

    #region Private

    #endregion
}