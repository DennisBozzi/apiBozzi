using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Dtos;

namespace apiBozzi.Models.FelicianoBozzi;

public class TenantDemo : Person
{
    [MaxLength(100)] [EmailAddress] public string? Email { get; set; }
    [MaxLength(15)] public string? Phone { get; set; }
    public DateTime? Born { get; set; }
    public TenantDemo? Responsible { get; set; }

    public TenantDemo()
    {
    }

    public TenantDemo(NewTenant dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Cpf = dto.Cpf;
        Email = dto.Email;
        Phone = dto.Phone;
        Born = dto.Born;
    }
}