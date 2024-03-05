using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class addActiveToMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "dbo",
                table: "MenuItems",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                schema: "dbo",
                table: "MenuItems");
        }
    }
}
