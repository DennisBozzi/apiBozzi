using System.Text.Json.Serialization;
using apiBozzi.Models.FelicianoBozzi;

namespace apiBozzi.Models.Dtos;

public class NewContract
{
    public DateTime ValidUntil { get; set; }
    public int PaymnetDay { get; set; }
    public int TenantId { get; set; }
    public int UnitId { get; set; }
    public decimal Rent { get; set; }
    [JsonIgnore] public Tenant? Tenant { get; set; }
    [JsonIgnore] public Unit? Unit { get; set; }
}