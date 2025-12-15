using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ClerkUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogIn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ClerkUserId);
                });

            migrationBuilder.CreateTable(
                name: "CasinoTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClerkUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserClerkUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasinoTransactions_Users_UserClerkUserId",
                        column: x => x.UserClerkUserId,
                        principalTable: "Users",
                        principalColumn: "ClerkUserId");
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClerkUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserClerkUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_UserClerkUserId",
                        column: x => x.UserClerkUserId,
                        principalTable: "Users",
                        principalColumn: "ClerkUserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTransactions_ClerkUserId_CreatedAt",
                table: "CasinoTransactions",
                columns: new[] { "ClerkUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoTransactions_UserClerkUserId",
                table: "CasinoTransactions",
                column: "UserClerkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserClerkUserId",
                table: "GameSessions",
                column: "UserClerkUserId");

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
                name: "Users");
        }
    }
}
