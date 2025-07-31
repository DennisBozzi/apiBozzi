using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Apartment
    {
        [Key] public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        [Required] [MaxLength(10)] public string Number { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Rent { get; set; }

        [Required] public FloorEnum Floor { get; set; }
        [Required] public ApartmentTypeEnum Type { get; set; }
        public Tenant? Responsible { get; set; }
    }
}