using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoApi.Migrations
{
    /// <inheritdoc />
    public partial class welcomeBonus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasClaimedWelcomeBonus",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasClaimedWelcomeBonus",
                table: "Users");
        }
    }
}
