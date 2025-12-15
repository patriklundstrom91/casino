public class BlackjackGameData
{
    public List<int> PlayerHand { get; set; } = new();
    public List<int> DealerVisible { get; set; } = new();
    public List<int>? DealerHand { get; set; }
    public string Status { get; set; }
}