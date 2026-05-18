using Microsoft.EntityFrameworkCore;
using PlayerService.API.Models;

namespace PlayerService.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Player> Players => Set<Player>();
}