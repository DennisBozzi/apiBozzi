using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class InquilinoApartamento
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InquilinoId { get; set; }
        
        [Required]
        public int ApartamentoId { get; set; }
        
        [Required]
        public DateTime DataInicio { get; set; }
        
        public DateTime? DataFim { get; set; }
        
        [ForeignKey("InquilinoId")]
        public virtual Inquilino Inquilino { get; set; }
        
        [ForeignKey("ApartamentoId")]
        public virtual Apartamento Apartamento { get; set; }
    }
}