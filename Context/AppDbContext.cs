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
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<ApartmentDemo> ApartmentsDemo { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantDemo> TenantsDemo { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Contract> Contracts { get; set; }
}