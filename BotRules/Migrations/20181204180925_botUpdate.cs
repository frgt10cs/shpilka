using Microsoft.EntityFrameworkCore.Migrations;

namespace BotRules.Migrations
{
    public partial class botUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BotCount",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotCount",
                table: "Users");
        }
    }
}
