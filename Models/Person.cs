using System.ComponentModel.DataAnnotations;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models;

public abstract class Person
{
    [Key] public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    [Required] [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required] [MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(11)] public string Cpf { get; set; } = string.Empty;
    [Required] public Gender Gender { get; set; } = Gender.Male;
}