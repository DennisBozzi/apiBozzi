using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Dtos;
using apiBozzi.Models.Enums;
using apiBozzi.Utils;

namespace apiBozzi.Models.FelicianoBozzi;

public class Tenant : Person
{
    [MaxLength(100)] [EmailAddress] public string? Email { get; set; }
    [MaxLength(15)] public string? Phone { get; set; }
    public DateTime? Born { get; set; }
    public MaritalStatus MaritalStatus { get; set; }

    public Tenant()
    {
    }

    public Tenant(NewTenant dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Cpf = dto.Cpf;
        Email = dto.Email;
        Phone = dto.Phone;
        Gender = dto.Gender;
        Born = dto.Born.ToUtcDateTime();
        MaritalStatus = dto.MaritalStatus;
    }
}