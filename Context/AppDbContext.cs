using apiBozzi.Models.FelicianoBozzi;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Feliciano Bozzi
    public DbSet<Inquilino> Inquilinos { get; set; }
    public DbSet<Dependente> Dependentes { get; set; }
    public DbSet<Apartamento> Apartamentos { get; set; }
    public DbSet<InquilinoApartamento> InquilinosApartamentos { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
}