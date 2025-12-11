
namespace CasinoApi;

public class User
{
    public string ClerkUserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public decimal Balance { get; set; } = 1000.00m;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastLogIn { get; set; } = DateTime.UtcNow;

    public ICollection<CasinoTransaction> CasinoTransactions { get; set; }
    public ICollection<GameSession> GameSessions { get; set; }
}
