using System.Text.Json;
using CasinoApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private readonly CasinoDbContext _context;
    private readonly ILogger<GamesController> _logger;
    private static Random _random = new();
    private int DrawCard() => Enumerable.Range(2, 10).Append(10).Append(10).Append(10).Append(11).ElementAt(_random.Next(15));
    private int GetHandValue(List<int> hand)
    {
        int value = hand.Sum();
        int aces = hand.Count(x => x == 11);
        while (value > 21 && aces > 0) { value -= 10; aces--; }
        return value;
    }
    public GamesController(CasinoDbContext context, ILogger<GamesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("slots/spin")]
    [Authorize]
    public async Task<ActionResult<SlotsResult>> SpinSlots([FromBody] SlotsSpinRequest request)
    {
        Console.WriteLine($"REQUEST: clerkUserId={request.ClerkUserId}, bet={request.Bet}");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == request.ClerkUserId);
        Console.WriteLine($"FOUND USER: {user?.ClerkUserId}");
        if (user == null) return NotFound("Did not find user in database");
        if (user.Balance < request.Bet) return BadRequest("Insufficient balance");

        user.Balance -= request.Bet;

        var symbols = new[] { "Cherry", "Lemon", "Bell", "Seven", "Diamond", "Star" };
        var result = new List<string>();
        var winMultiplier = 0m;

        for (int i = 0; i < 3; i++)
        {
            result.Add(symbols[new Random().Next(symbols.Length)]);
        }

        if (result[0] == result[1] && result[1] == result[2])
        {
            winMultiplier = result[0] == "Diamond" ? 50m : result[0] == "Seven" ? 20m : 5m;
        }

        var winAmount = request.Bet * winMultiplier;
        user.Balance += winAmount;

        var session = new GameSession
        {
            ClerkUserId = request.ClerkUserId,
            GameType = "Slots",
            BetAmount = request.Bet,
            WinAmount = winAmount,
            Result = JsonSerializer.Serialize(result),
            PlayedAt = DateTime.UtcNow
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        return new SlotsResult
        {
            ClerkUserId = user.ClerkUserId,
            Symbols = result,
            WinAmount = winAmount,
            NewBalance = user.Balance
        };
    }

    [HttpPost("blackjack/join")]
    [Authorize]
    public async Task<ActionResult<BlackjackResult>> JoinBlackjack([FromBody] BlackjackJoinRequest request)
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);
        if (user == null) return NotFound("Did not find user in database");
        if (user.Balance < request.Bet) return BadRequest("Insufficient balance");

        user.Balance -= request.Bet;

        // Draw cards (2-11 where 11=Ace)
        var playerHand = new List<int> { DrawCard(), DrawCard() };
        var dealerHand = new List<int> { DrawCard(), DrawCard() };

        var gameStatus = GetHandValue(playerHand) == 21 ? "PlayerBlackjack" : "InProgress";

        var session = new GameSession
        {
            ClerkUserId = clerkUserId!,
            GameType = "Blackjack",
            BetAmount = request.Bet,
            WinAmount = 0,
            Result = JsonSerializer.Serialize(new { playerHand, dealerVisible = new[] { dealerHand[0] }, status = gameStatus }),
            PlayedAt = DateTime.UtcNow
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        return new BlackjackResult
        {
            ClerkUserId = user.ClerkUserId,
            PlayerHand = playerHand,
            DealerVisible = new List<int> { dealerHand[0] },
            Status = gameStatus,
            NewBalance = user.Balance
        };
    }

    [HttpPost("blackjack/{sessionId}/hit")]
    [Authorize]
    public async Task<ActionResult<BlackjackResult>> HitBlackjack(int sessionId, [FromBody] BlackjackActionRequest request)
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var session = await _context.GameSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClerkUserId == clerkUserId && s.GameType == "Blackjack");

        if (session == null || session.Result == null) return BadRequest("Invalid session");

        var gameData = JsonSerializer.Deserialize<BlackjackGameData>(session.Result);
        if (gameData.Status != "InProgress") return BadRequest("Game not in progress");

        gameData.PlayerHand.Add(DrawCard());
        gameData.Status = GetHandValue(gameData.PlayerHand) > 21 ? "PlayerBust" : "InProgress";

        var user = await _context.Users.FirstAsync(u => u.ClerkUserId == clerkUserId);
        session.Result = JsonSerializer.Serialize(gameData);

        await _context.SaveChangesAsync();

        return new BlackjackResult
        {
            ClerkUserId = user.ClerkUserId,
            PlayerHand = gameData.PlayerHand,
            DealerVisible = gameData.DealerVisible,
            Status = gameData.Status,
            NewBalance = user.Balance
        };
    }

    [HttpPost("blackjack/{sessionId}/stand")]
    [Authorize]
    public async Task<ActionResult<BlackjackResult>> StandBlackjack(int sessionId, [FromBody] BlackjackActionRequest request)
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var session = await _context.GameSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClerkUserId == clerkUserId && s.GameType == "Blackjack");

        if (session == null || session.Result == null) return BadRequest("Invalid session");

        var gameData = JsonSerializer.Deserialize<BlackjackGameData>(session.Result);
        if (gameData.Status != "InProgress") return BadRequest("Game not in progress");

        var dealerHand = new List<int> { gameData.DealerVisible[0], DrawCard() };
        while (GetHandValue(dealerHand) < 17) dealerHand.Add(DrawCard());

        gameData.DealerHand = dealerHand;

        var playerValue = GetHandValue(gameData.PlayerHand);
        var dealerValue = GetHandValue(dealerHand);

        decimal winAmount = 0;
        var user = await _context.Users.FirstAsync(u => u.ClerkUserId == clerkUserId);
        decimal bet = session.BetAmount;

        if (gameData.Status == "PlayerBlackjack") winAmount = bet * 1.5m;
        else if (dealerValue > 21 || playerValue > dealerValue) winAmount = bet * 2;
        else if (playerValue == dealerValue) winAmount = bet;

        if (winAmount > 0) user.Balance += winAmount;
        session.WinAmount = winAmount;
        gameData.Status = winAmount > 0 ? "PlayerWin" : playerValue > 21 ? "PlayerBust" : "PlayerLose";
        gameData.DealerHand = dealerHand;

        session.Result = JsonSerializer.Serialize(gameData);
        await _context.SaveChangesAsync();

        return new BlackjackResult
        {
            ClerkUserId = user.ClerkUserId,
            PlayerHand = gameData.PlayerHand,
            DealerHand = dealerHand,
            Status = gameData.Status,
            WinAmount = winAmount,
            NewBalance = user.Balance
        };
    }
}