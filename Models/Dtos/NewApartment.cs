using System.ComponentModel.DataAnnotations;

namespace apiBozzi.Models.Dtos;

public class NewApartment
{
    [Required] public string Number { get; set; }
    [Required] public int Floor { get; set; }
    [Required] public int Type { get; set; }
    [Required] public decimal RentValue { get; set; }
}