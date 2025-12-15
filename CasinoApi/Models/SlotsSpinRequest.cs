using System.ComponentModel.DataAnnotations;

public class SlotsSpinRequest
{
    [Key]
    public string ClerkUserId { get; set; } = string.Empty;
    public decimal Bet { get; set; }
}