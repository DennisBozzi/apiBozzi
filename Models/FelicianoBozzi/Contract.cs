using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi;

public class Contract
{
    [Key] public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime ValidUntil { get; set; }
    public int PaymnetDay { get; set; }
    public StatusContract Status { get; set; }
    public File File { get; set; }
    [Required] public Tenant? Responsible { get; set; }
    [Required] public Unit Unit { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Rent { get; set; }
}