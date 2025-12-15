
using System.ComponentModel.DataAnnotations;

namespace CasinoApi;

public class User
{
    [Key]
    public string ClerkUserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public decimal Balance { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastLogIn { get; set; } = DateTime.UtcNow;
    public bool HasClaimedWelcomeBonus { get; set; } = false;

    public ICollection<CasinoTransaction> CasinoTransactions { get; set; }
    public ICollection<GameSession> GameSessions { get; set; }
}
