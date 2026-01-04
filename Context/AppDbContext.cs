using apiBozzi.Models;
using apiBozzi.Models.FelicianoBozzi;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Context;

using File = apiBozzi.Models.File;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var navigation in entityType.GetNavigations())
            {
                navigation.SetIsEagerLoaded(true);
            }
        }
    }

    // Feliciano Bozzi
    public DbSet<File> Files { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
}