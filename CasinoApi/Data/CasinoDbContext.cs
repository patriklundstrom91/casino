using CasinoApi;
using Microsoft.EntityFrameworkCore;

public class CasinoDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CasinoTransaction> CasinoTransactions { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.ClerkUserId).IsUnique();

        modelBuilder.Entity<CasinoTransaction>().HasIndex(t => new { t.ClerkUserId, t.CreatedAt });
    }
}