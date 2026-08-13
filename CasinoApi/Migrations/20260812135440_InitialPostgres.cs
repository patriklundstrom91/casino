using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CasinoApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "SlotsResults",
                columns: table => new
                {
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    Symbols = table.Column<string>(type: "jsonb", nullable: false),
                    WinAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NewBalance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotsResults", x => x.ClerkUserId);
                });

            migrationBuilder.CreateTable(
                name: "SlotsSpinRequests",
                columns: table => new
                {
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    Bet = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotsSpinRequests", x => x.ClerkUserId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    LastLogIn = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    HasClaimedWelcomeBonus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ClerkUserId);
                });

            migrationBuilder.CreateTable(
                name: "CasinoTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    GameType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTransactions_Users_ClerkUserId",
                        column: x => x.ClerkUserId,
                        principalTable: "Users",
                        principalColumn: "ClerkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    GameType = table.Column<string>(type: "text", nullable: false),
                    BetAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    WinAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_ClerkUserId",
                        column: x => x.ClerkUserId,
                        principalTable: "Users",
                        principalColumn: "ClerkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTransactions_ClerkUserId_CreatedAt",
                table: "CasinoTransactions",
                columns: new[] { "ClerkUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_ClerkUserId",
                table: "GameSessions",
                column: "ClerkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClerkUserId",
                table: "Users",
                column: "ClerkUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoTransactions");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "SlotsResults");

            migrationBuilder.DropTable(
                name: "SlotsSpinRequests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
