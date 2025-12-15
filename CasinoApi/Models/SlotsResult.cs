using System.ComponentModel.DataAnnotations;

public class SlotsResult
{
    [Key]
    public string ClerkUserId { get; set; } = string.Empty;
    public List<string> Symbols { get; set; } = new();
    public decimal WinAmount { get; set; }
    public decimal NewBalance { get; set; }
}