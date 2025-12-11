
namespace CasinoApi;

public class CasinoTransaction
{
    public int Id { get; set; }
    public string ClerkUserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
