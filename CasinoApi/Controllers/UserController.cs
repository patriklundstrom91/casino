using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CasinoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly CasinoDbContext _context;
    public UserController(CasinoDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var fullName = User.FindFirst("fullname")?.Value;
        var name = User.FindFirst("name")?.Value;

        if (string.IsNullOrEmpty(clerkUserId)) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        if (user == null)
        {
            user = new User
            {
                ClerkUserId = clerkUserId,
                UserName = name ?? fullName ?? email?.Split("@")[0] ?? "player"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ New user created: {user.ClerkUserId} / {user.UserName}");
        }

        return new UserDto
        {
            UserName = user.UserName,
            Balance = user.Balance,
            TotalTransactions = await _context.CasinoTransactions.CountAsync(t => t.ClerkUserId == clerkUserId)
        };
    }

    [Authorize]
    [HttpPost("balance")]
    public async Task<ActionResult> UpdateBalance([FromBody] UpdateBalanceDto dto)
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        if (clerkUserId == null) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);
        if (user == null) return NotFound();

        user.Balance += dto.Amount;
        user.LastLogIn = DateTime.UtcNow;

        var transaction = new CasinoTransaction
        {
            ClerkUserId = clerkUserId,
            Amount = dto.Amount,
            Type = dto.Amount > 0 ? "win" : "loss",
            GameType = dto.GameType,
            CreatedAt = DateTime.UtcNow
        };

        _context.CasinoTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(new { balance = user.Balance });
    }

    [HttpPost("claim-bonus")]
    [Authorize]
    public async Task<ActionResult> ClaimWelcomeBonus()
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        Console.WriteLine("Backend clerk Id: " + clerkUserId);
        if (string.IsNullOrEmpty(clerkUserId))
        {
            Console.WriteLine("ALL CLAIMS:");
            foreach (var claim in User.Claims)
                Console.WriteLine($"  {claim.Type}: {claim.Value}");
            return Unauthorized("No user ID in token");
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        if (user == null) return NotFound("User not found");

        if (user.HasClaimedWelcomeBonus)
            return BadRequest(new { error = "Welcome bonus already claimed!" });

        const decimal bonusAmount = 1000.00m;

        user.Balance += bonusAmount;
        user.HasClaimedWelcomeBonus = true;
        user.LastLogIn = DateTime.UtcNow;

        var transaction = new CasinoTransaction
        {
            ClerkUserId = clerkUserId!,
            Amount = bonusAmount,
            Type = "Deposit",
            GameType = "welcome_bonus",
            CreatedAt = DateTime.UtcNow
        };
        _context.CasinoTransactions.Add(transaction);

        await _context.SaveChangesAsync();

        return Ok(new { balance = user.Balance, message = $"Welcome Bonus +${bonusAmount} claimed!" });
    }

    [HttpGet("account")]
    [Authorize]
    public async Task<ActionResult<AccountInfoDto>> GetAccount()
    {
        var clerkUserId = User.FindFirst("sub")?.Value;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        var sessions = await _context.GameSessions
            .Where(s => s.ClerkUserId == clerkUserId)
            .OrderByDescending(s => s.PlayedAt)
            .Take(20)
            .Select(s => new GameSessionDto
            {
                Id = s.Id,
                GameType = s.GameType,
                Bet = s.BetAmount,
                WinAmount = s.WinAmount,
                NetProfit = s.WinAmount - s.BetAmount,
                Result = s.Result,
                PlayedAt = s.PlayedAt
            })
            .ToListAsync();

        var transactions = await _context.CasinoTransactions
            .Where(t => t.ClerkUserId == clerkUserId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new CasinoTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                GameType = t.GameType,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return new AccountInfoDto
        {
            ClerkUserId = user.ClerkUserId,
            UserName = user.UserName,
            Balance = user.Balance,
            GameSessions = sessions,
            CasinoTransactions = transactions
        };
    }

}
