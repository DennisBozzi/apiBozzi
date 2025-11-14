using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi;

public class Contract
{
    [Key] public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Rent { get; set; }

    public TenantDemo? Responsible { get; set; }
    [Required] public required File File { get; set; }
}