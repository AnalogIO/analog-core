using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseTypeNonNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
