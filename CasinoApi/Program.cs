using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://proud-forest-083b0be03.7.azurestaticapps.net",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddDbContext<CasinoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// ⭐ FÅNGA ALLA OPTIONS
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok());

// ⭐ CORS MÅSTE LIGGA EFTER ROUTING
app.UseCors();

// ⭐ AUTH
app.UseAuthentication();
app.UseAuthorization();

// ⭐ CONTROLLERS
app.MapControllers();

app.Run();
