using System.Text.Json;
using CasinoApi;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("games/[action]")]
public class GamesController : ControllerBase
{
    private readonly CasinoDbContext _context;
    private readonly ILogger<GamesController> _logger;
    public GamesController(CasinoDbContext context, ILogger<GamesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("slots/spin")]
    public async Task<ActionResult<SlotsResult>> SpinSlots([FromBody] SlotsSpinRequest request)
    {
        var user = await _context.Users.FirstAsync(u => u.ClerkUserId == request.ClerkUserId);

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
            Symbols = result,
            winAmount = winAmount,
            NewBalance = user.Balance
        };
    }
}