public class WithdrawResponseDto
{
    public int TransactionId { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime Timestamp { get; set; }
}