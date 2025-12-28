using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.Dtos;

public class NewTenant
{
    [Required] public string? FirstName { get; set; }
    [Required] public string? LastName { get; set; }
    [Required] public string? Cpf { get; set; }
    [Required] public string? Email { get; set; }
    [Required] public Gender Gender { get; set; }
    public DateTime Born { get; set; }
    public string? Phone { get; set; }
}