public class CasinoTransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = "";
    public string GameType { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}