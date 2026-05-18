using Microsoft.EntityFrameworkCore;
using GameService.API.Models;

namespace GameService.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Game> Games => Set<Game>();
}