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
    private string GetUserName(string? firstName, string? fullName, string? email)
    {
        return firstName ?? fullName ?? email?.Split("@")[0] ?? "player";
    }

    [HttpGet("{clerkUserId}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var firstName = User.FindFirst("name")?.Value;
        var fullName = User.FindFirst("fullname")?.Value;
        var email = User.FindFirst("email")?.Value;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        if (user == null)
        {
            user = new User
            {
                ClerkUserId = clerkUserId!,
                UserName = GetUserName(firstName, fullName, email)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return new UserDto
        {
            GetUserName = user.UserName,
            Balance = user.Balance,
            TotalTransactions = await _context.CasinoTransactions.CountAsync(t => t.ClerkUserId == clerkUserId)
        };
    }

    [HttpPost("balance/{clerkUserId}/{amount}")]
    public async Task<ActionResult> UpdateBalance(decimal amount, [FromQuery] string gameType)
    {
        var clerkUserId = User.FindFirst("sub")?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);
        if (user == null) return NotFound();
        user.Balance += amount;
        user.LastLogIn = DateTime.UtcNow;

        var transaction = new CasinoTransaction
        {
            ClerkUserId = clerkUserId!,
            Amount = amount,
            Type = amount > 0 ? "win" : "loss",
            GameType = gameType,
            CreatedAt = DateTime.UtcNow
        };
        _context.CasinoTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(new { balance = user.Balance });
    }
}
