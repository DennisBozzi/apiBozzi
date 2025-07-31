using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Enums;
using apiBozzi.Utils;

namespace apiBozzi.Models.Responses;

public class ApartmentResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Number { get; set; }
    public decimal Rent { get; set; }
    public FloorEnum Floor { get; set; }
    public ApartmentTypeEnum Type { get; set; }
    public TenantResponse? Responsible { get; set; }
    public virtual ICollection<TenantResponse> Residents { get; set; } = new List<TenantResponse>();

    public ApartmentResponse()
    {
    }

    public ApartmentResponse(Apartment value)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        Number = value.Number;
        Rent = value.Rent;
        Floor = value.Floor;
        Type = value.Type;
        Responsible = value.Responsible.HasValue() ? new TenantResponse(value.Responsible) : null;
    }

    public ApartmentResponse WithResidents(ICollection<Tenant> value)
    {
        Residents = value.Select(x => new TenantResponse(x, false)).ToList();
        return this;
    }
}