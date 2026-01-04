using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;

namespace apiBozzi.Models.Responses;

public class ContractResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime ValidSince { get; set; }
    public DateTime ValidUntil { get; set; }
    public int PaymentDay { get; set; }
    public StatusContract Status { get; set; }
    public File? File { get; set; }
    public TenantResponse? Tenant { get; set; }
    public UnitResponse? Unit { get; set; }
    public decimal Rent { get; set; }

    public ContractResponse()
    {
    }

    public ContractResponse(Contract contract)
    {
        Id = contract.Id;
        CreatedAt = contract.CreatedAt;
        ValidSince = contract.ValidSince;
        ValidUntil = contract.ValidUntil;
        PaymentDay = contract.PaymentDay;
        Status = contract.Status;
        File = contract.File;
        Tenant = new TenantResponse(contract.Tenant);
        Unit = new UnitResponse(contract.Unit);
        Rent = contract.Rent;
    }
}