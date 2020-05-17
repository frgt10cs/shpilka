using Microsoft.EntityFrameworkCore.Migrations;

namespace BotRules.Migrations
{
    public partial class sessiontopUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "CurrentCommandFunction",
                table: "Sessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCommandFunction",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
