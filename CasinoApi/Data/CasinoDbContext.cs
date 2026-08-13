using CasinoApi;
using Microsoft.EntityFrameworkCore;

public class CasinoDbContext : DbContext
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CasinoTransaction> CasinoTransactions { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<SlotsSpinRequest> SlotsSpinRequests { get; set; }
    public DbSet<SlotsResult> SlotsResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.ClerkUserId);

            entity.Property(u => u.ClerkUserId).HasColumnType("text");
            entity.Property(u => u.UserName).HasColumnType("text");
            entity.Property(u => u.Balance).HasColumnType("numeric");
            entity.Property(u => u.CreatedAt).HasColumnType("timestamptz");
            entity.Property(u => u.LastLogIn).HasColumnType("timestamptz");
            entity.Property(u => u.HasClaimedWelcomeBonus).HasColumnType("boolean");

            entity.HasIndex(u => u.ClerkUserId).IsUnique();
        });

        // CASINO TRANSACTION
        modelBuilder.Entity<CasinoTransaction>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.ClerkUserId).HasColumnType("text");
            entity.Property(t => t.Amount).HasColumnType("numeric");
            entity.Property(t => t.Type).HasColumnType("text");
            entity.Property(t => t.GameType).HasColumnType("text");
            entity.Property(t => t.CreatedAt).HasColumnType("timestamptz");

            entity.HasIndex(t => new { t.ClerkUserId, t.CreatedAt });

            entity.HasOne<User>()
                  .WithMany(u => u.CasinoTransactions)
                  .HasForeignKey(t => t.ClerkUserId)
                  .HasPrincipalKey(u => u.ClerkUserId);
        });

        // GAME SESSION
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.ClerkUserId).HasColumnType("text");
            entity.Property(g => g.GameType).HasColumnType("text");
            entity.Property(g => g.BetAmount).HasColumnType("numeric");
            entity.Property(g => g.WinAmount).HasColumnType("numeric");
            entity.Property(g => g.Result).HasColumnType("text");
            entity.Property(g => g.PlayedAt).HasColumnType("timestamptz");

            entity.HasOne<User>()
                  .WithMany(u => u.GameSessions)
                  .HasForeignKey(g => g.ClerkUserId)
                  .HasPrincipalKey(u => u.ClerkUserId);
        });

        // SLOTS SPIN REQUEST
        modelBuilder.Entity<SlotsSpinRequest>(entity =>
        {
            entity.HasKey(s => s.ClerkUserId);

            entity.Property(s => s.ClerkUserId).HasColumnType("text");
            entity.Property(s => s.Bet).HasColumnType("numeric");
        });

        // SLOTS RESULT
        modelBuilder.Entity<SlotsResult>(entity =>
        {
            entity.HasKey(s => s.ClerkUserId);

            entity.Property(s => s.ClerkUserId).HasColumnType("text");
            entity.Property(s => s.WinAmount).HasColumnType("numeric");
            entity.Property(s => s.NewBalance).HasColumnType("numeric");

            // List<string> → JSONB
            entity.Property(s => s.Symbols)
                  .HasColumnType("jsonb");
        });
    }
}
