
using Demo_Webapplication_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo_Webapplication_1.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
