using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace CasinoApi.Controllers;

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly CasinoDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        CasinoDbContext db,
        IConfiguration config,
        ILogger<PaymentsController> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    // ---------------------------------------------------------
    // 1. Create Stripe Checkout Session
    // ---------------------------------------------------------
    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] DepositRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized("Missing Clerk user ID");

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            ClientReferenceId = userId,
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "USD",
                        UnitAmount = request.Amount * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Casino Deposit"
                        }
                    },
                    Quantity = 1
                }
            },
            SuccessUrl = _config["Stripe:SuccessUrl"],
            CancelUrl = _config["Stripe:CancelUrl"]
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Ok(new { url = session.Url });
    }

    // ---------------------------------------------------------
    // 2. Stripe Webhook
    // ---------------------------------------------------------
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];
        var secret = _config["Stripe:WebhookSecret"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe signature validation failed");
            return BadRequest("Invalid Stripe signature");
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null)
            {
                _logger.LogError("Stripe session object was null");
                return Ok();
            }

            var userId = session.ClientReferenceId;
            var amount = (decimal)(session.AmountTotal ?? 0) / 100m;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.ClerkUserId == userId);
            if (user == null)
            {
                _logger.LogWarning("Webhook received for non-existing user: {UserId}", userId);
                return Ok();
            }

            user.Balance += amount;

            _db.CasinoTransactions.Add(new CasinoTransaction
            {
                ClerkUserId = userId,
                Amount = amount,
                Type = "Deposit",
                GameType = "stripe",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            _logger.LogInformation("Deposit completed for user {UserId}, amount {Amount}", userId, amount);
        }

        return Ok();
    }

    // ---------------------------------------------------------
    // 3. Withdraw
    // ---------------------------------------------------------
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized("Missing Clerk user ID");

        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than zero");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.ClerkUserId == userId);
        if (user == null)
            return NotFound("User not found");

        if (user.Balance < request.Amount)
            return BadRequest("Insufficient balance");

        user.Balance -= request.Amount;

        _db.CasinoTransactions.Add(new CasinoTransaction
        {
            ClerkUserId = userId,
            Amount = -request.Amount,
            Type = "Withdraw",
            GameType = "Account",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return Ok(new { balance = user.Balance });
    }
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
}

public class DepositRequest
{
    public int Amount { get; set; }
}
