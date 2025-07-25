using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Inquilino
    {
        [Key] public int Id { get; set; }

        [Required] [MaxLength(100)] public string Nome { get; set; }

        [Required] [MaxLength(11)] public string CPF { get; set; }

        [MaxLength(100)] [EmailAddress] public string? Email { get; set; }

        [MaxLength(15)] public string? Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public virtual ICollection<Dependente> Dependentes { get; set; } = new List<Dependente>();

        public virtual ICollection<InquilinoApartamento> InquilinosApartamentos { get; set; } =
            new List<InquilinoApartamento>();
    }
}