using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Services (INORDNING!)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS FÖRST
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://patriklundstrom91.github.io",
            "https://patriklundstrom91.github.io/casino",
            "http://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // För JWT cookies
    });
});

// DbContext
builder.Services.AddDbContext<CasinoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT SIST bland services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Casino-JWT", options =>
{
    options.Authority = "https://model-servant-37.clerk.accounts.dev";  // ← BYT denna!
    options.Audience = "casino-api";
    options.RequireHttpsMetadata = false;  // Azure
});


builder.Services.AddAuthorization();

var app = builder.Build();

// MIDDLEWARE ORDNING (KRITISK!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();      // 1. CORS
app.UseAuthentication();         // 2. AUTHENTICATION 
app.UseAuthorization();          // 3. AUTHORIZATION
app.MapControllers();            // 4. CONTROLLERS SIST!

app.Run();
