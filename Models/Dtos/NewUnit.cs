using System.ComponentModel.DataAnnotations;

namespace apiBozzi.Models.Dtos;

public class NewUnit
{
    [Required] public string Number { get; set; }
    [Required] public int Floor { get; set; }
    [Required] public int Type { get; set; }
}