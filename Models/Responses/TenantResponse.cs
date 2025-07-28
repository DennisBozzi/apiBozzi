using apiBozzi.Models.FelicianoBozzi;

namespace apiBozzi.Models.Responses;

public class TenantResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? Born { get; set; }
    public Tenant? Responsible { get; set; }
    public string Number { get; set; }
    public virtual ICollection<Tenant> Dependents { get; set; } = new List<Tenant>();
    
    public TenantResponse(Tenant value)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        FirstName = value.FirstName;
        LastName = value.LastName;
        Cpf = value.Cpf;
        Email = value.Email;
        Phone = value.Phone;
        Born = value.Born;
        Responsible = value.Responsible;
    }

    public TenantResponse WithApartment(Apartment ap)
    {
        Number = ap.Number;
        return this;
    }
}