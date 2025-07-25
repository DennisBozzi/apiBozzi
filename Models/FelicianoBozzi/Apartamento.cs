using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apiBozzi.Models.Enums;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Apartamento
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Numero { get; set; }
        
        public AndarEnum Andar { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorAluguel { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now.ToUniversalTime();
        
        public virtual ICollection<InquilinoApartamento> InquilinosApartamentos { get; set; } = new List<InquilinoApartamento>();
        public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    }
}