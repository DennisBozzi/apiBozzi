using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Enums;
using apiBozzi.Utils;

namespace apiBozzi.Models.Responses;

public class UnitResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Number { get; set; }
    public Floor Floor { get; set; }
    public UnitType Type { get; set; }
    public TenantResponse? Responsible { get; set; }
    public decimal Rent { get; set; }

    public UnitResponse()
    {
    }

    public UnitResponse(Unit value)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        Number = value.Number;
        Floor = value.Floor;
        Type = value.Type;
    }

    public void FillContract(Contract value)
    {
        Rent = value.Rent;
        Responsible = new TenantResponse(value.Tenant);
    }
}