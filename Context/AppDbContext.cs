using apiBozzi.Models;
using apiBozzi.Models.FelicianoBozzi;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Context;

using File = apiBozzi.Models.File;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Feliciano Bozzi
    public DbSet<File> Files { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
}