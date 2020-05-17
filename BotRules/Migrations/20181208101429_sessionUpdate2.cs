using Microsoft.EntityFrameworkCore.Migrations;

namespace BotRules.Migrations
{
    public partial class sessionUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BotId",
                table: "Sessions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotId",
                table: "Sessions");
        }
    }
}
