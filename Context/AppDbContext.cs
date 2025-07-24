using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}