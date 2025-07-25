using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiBozzi.Models.FelicianoBozzi
{
    public class Pagamento
    {
        [Key] public int Id { get; set; }

        [Required] public int ApartamentoId { get; set; }

        [Required] public DateTime MesReferencia { get; set; }

        [Column(TypeName = "decimal(10,2)")] public decimal? ValorPago { get; set; }

        [Required] public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

        [MaxLength(20)] public string Status { get; set; } = "Pendente";

        [MaxLength(500)] public string? Observacoes { get; set; }
        
        [ForeignKey("ApartamentoId")] public virtual Apartamento Apartamento { get; set; }
    }
}