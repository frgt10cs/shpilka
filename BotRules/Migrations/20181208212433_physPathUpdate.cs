using Microsoft.EntityFrameworkCore.Migrations;

namespace BotRules.Migrations
{
    public partial class physPathUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhysicalPath",
                table: "Bots",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhysicalPath",
                table: "Bots");
        }
    }
}
