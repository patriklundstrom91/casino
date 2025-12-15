public class AccountInfoDto
{
    public string ClerkUserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public decimal Balance { get; set; }
    public List<GameSessionDto> GameSessions { get; set; } = new();
    public List<CasinoTransactionDto> CasinoTransactions { get; set; } = new();
}