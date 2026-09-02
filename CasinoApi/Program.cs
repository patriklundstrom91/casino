using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Stripe API-nyckel
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://proud-forest-083b0be03.7.azurestaticapps.net",
            "http://localhost:5173",
            "http://localhost"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// DB
builder.Services.AddDbContext<CasinoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AUTH (Clerk)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://model-serval-37.clerk.accounts.dev";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://model-serval-37.clerk.accounts.dev",

            ValidateAudience = true,
            ValidAudience = "casino-api",

            ValidateLifetime = true
        };

        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routing måste komma innan CORS + Auth
app.UseRouting();

// Fånga OPTIONS (fix för Azure SWA)
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok());

// CORS
app.UseCors();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
