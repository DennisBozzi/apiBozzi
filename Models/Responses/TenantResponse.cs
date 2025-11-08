using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Utils;

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
    public TenantResponse? Responsible { get; set; }
    public ApartmentResponse? Apartment { get; set; }
    public virtual ICollection<TenantResponse> Dependents { get; set; } = new List<TenantResponse>();

    public TenantResponse(Tenant value, bool useResponsible = true)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        FirstName = value.FirstName;
        LastName = value.LastName;
        Cpf = value.Cpf;
        Email = value.Email;
        Phone = value.Phone;
        Born = value.Born;
        Responsible = value.Responsible != null && useResponsible ? new TenantResponse(value.Responsible, false) : null;
    }

    public TenantResponse(TenantDemo value, bool useResponsible = true)
    {
        Id = value.Id;
        CreatedAt = value.CreatedAt;
        FirstName = value.FirstName;
        LastName = value.LastName;
        Cpf = value.Cpf;
        Email = value.Email;
        Phone = value.Phone;
        Born = value.Born;
        Responsible = value.Responsible != null && useResponsible ? new TenantResponse(value.Responsible, false) : null;
    }

    public TenantResponse WithApartment(Apartment ap)
    {
        Apartment = new ApartmentResponse(ap);
        return this;
    }

    public TenantResponse WithApartment(ApartmentDemo ap)
    {
        Apartment = new ApartmentResponse(ap);
        return this;
    }

    public TenantResponse WithDependents(ICollection<Tenant> value)
    {
        Dependents = value.Select(x => new TenantResponse(x, false)).ToList();
        return this;
    }

    public TenantResponse WithDependents(ICollection<TenantDemo> value)
    {
        Dependents = value.Select(x => new TenantResponse(x, false)).ToList();
        return this;
    }
}