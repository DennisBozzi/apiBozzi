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
    public TenantResponse? Responsible { get; set; }

    public ApartmentResponse(Apartment value)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        Number = value.Number;
        Rent = value.Rent;
        Floor = value.Floor;
        Responsible = value.Responsible.HasValue() ? new TenantResponse(value.Responsible) : null;
    }
}