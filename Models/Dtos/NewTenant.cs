using System.ComponentModel.DataAnnotations;

namespace apiBozzi.Models.Dtos;

public class NewTenant
{
    [Required] public string? FirstName { get; set; }
    [Required] public string? LastName { get; set; }
    [Required] public string? Cpf { get; set; }
    [Required] public string? Email { get; set; }
    public DateTime Born { get; set; }
    public string? Phone { get; set; }
}