using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Dtos;

namespace apiBozzi.Models.FelicianoBozzi;

public class Tenant : Person
{
    [MaxLength(100)] [EmailAddress] public string? Email { get; set; }
    [MaxLength(15)] public string? Phone { get; set; }
    public DateTime? Born { get; set; }
    public Tenant? ResponsibleTenant { get; set; }
    public virtual ICollection<Tenant> Dependents { get; set; } = new List<Tenant>();

    public Tenant(){}
    
    public Tenant(NewTenant dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Cpf = dto.Cpf;
        Email = dto.Email;
        Phone = dto.Phone;
        Born = dto.Born;
    }
}