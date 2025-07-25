using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Dependente
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InquilinoId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
        
        [MaxLength(11)]
        public string? CPF { get; set; }
        
        public DateTime? DataNascimento { get; set; }
        
        [MaxLength(50)]
        public string? Parentesco { get; set; }
        
        [ForeignKey("InquilinoId")]
        public virtual Inquilino Inquilino { get; set; }
    }
}