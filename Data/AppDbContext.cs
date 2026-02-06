using Microsoft.EntityFrameworkCore;
using Motos.Models;

namespace Motos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Moto> Motos { get; set; }
}
