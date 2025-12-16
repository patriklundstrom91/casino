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
    private readonly Random _random = new();
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
}