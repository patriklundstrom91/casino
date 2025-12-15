public class GameSessionDto
{
    public int Id { get; set; }
    public string GameType { get; set; } = "";
    public decimal Bet { get; set; }  // BetAmount
    public decimal WinAmount { get; set; }
    public decimal NetProfit { get; set; }  // Beräknad!
    public string Result { get; set; } = "";
    public DateTime PlayedAt { get; set; }
}