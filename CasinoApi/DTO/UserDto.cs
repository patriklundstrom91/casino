public class UserDto
{
    public string UserName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int TotalTransactions { get; set; }
    public bool HasClaimedWelcomeBonus { get; set; } = false;
}