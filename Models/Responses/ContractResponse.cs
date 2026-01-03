using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;

namespace apiBozzi.Models.Responses;

public class ContractResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime ValidSince { get; set; }
    public DateTime ValidUntil { get; set; }
    public int PaymnetDay { get; set; }
    public StatusContract Status { get; set; }
    public File? File { get; set; }
    public Tenant? Tenant { get; set; }
    public Unit? Unit { get; set; }
    public decimal Rent { get; set; }

    public ContractResponse(Contract contract)
    {
        Id = contract.Id;
        CreatedAt = contract.CreatedAt;
        ValidSince = contract.ValidSince;
        ValidUntil = contract.ValidUntil;
        PaymnetDay = contract.PaymnetDay;
        Status = contract.Status;
        File = contract.File;
        Tenant = contract.Tenant;
        Unit = contract.Unit;
        Rent = contract.Rent;
    }
}