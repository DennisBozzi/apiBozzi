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
    }
}