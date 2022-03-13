using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class AddUserState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserState",
                schema: "dbo",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserState",
                schema: "dbo",
                table: "Users");
        }
    }
}
