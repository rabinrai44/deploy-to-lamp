using deploy_to_linux.Core.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace deploy_to_linux.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}