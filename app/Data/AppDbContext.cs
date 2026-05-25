using Microsoft.EntityFrameworkCore;

namespace _10xPV.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}