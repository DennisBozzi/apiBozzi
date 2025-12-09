using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Unit
    {
        [Key] public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        [Required] [MaxLength(10)] public string Number { get; set; } = string.Empty;
        [Required] public Floor Floor { get; set; }
        [Required] public UnitType Type { get; set; }
    }
}