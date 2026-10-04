using Microsoft.EntityFrameworkCore;
using WorldClock.Core;

namespace WorldClock.Maui.Data;

public class WorldClockDbContext : DbContext
{
    public DbSet<ClockLocation> Locations { get; set; }

    public WorldClockDbContext(
        DbContextOptions<WorldClockDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClockLocation>()
            .HasKey(location => location.City);
    }
}