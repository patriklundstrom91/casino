
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApi;

public class GameSession
{
    [Key]
    public int Id { get; set; }
    public string ClerkUserId { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public decimal BetAmount { get; set; }
    public decimal WinAmount { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
