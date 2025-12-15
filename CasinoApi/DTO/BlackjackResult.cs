public class BlackjackResult
{
    public string ClerkUserId { get; set; }
    public List<int> PlayerHand { get; set; }
    public List<int> DealerVisible { get; set; }
    public List<int>? DealerHand { get; set; }
    public string Status { get; set; }
    public decimal WinAmount { get; set; }
    public decimal NewBalance { get; set; }
}