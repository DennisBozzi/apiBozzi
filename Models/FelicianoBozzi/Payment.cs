using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi;

public class Payment
{
    [Key] public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime Competence { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Note { get; set; }
    public StatusPayment Status { get; set; }
    public PaymentType Type { get; set; }
    [Required] private Contract Contract { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Value { get; set; }
}