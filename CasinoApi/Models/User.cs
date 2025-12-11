
namespace CasinoApi;

public class User
{
    public string ClerkUserId { get; set; }

    public int UserName { get; set; }

    public int Balance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLogIn { get; set; }

    public ICollection<CasinoTransaction> CasinoTransactions { get; set; }
    public ICollection<GameSession> GameSessions { get; set; }
}
