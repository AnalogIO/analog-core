using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.WebApi.Migrations
{
    public partial class Vouchers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "test",
                table: "Vouchers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "test",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requester",
                schema: "test",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                schema: "test",
                table: "Vouchers",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Code",
                schema: "test",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "test",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Requester",
                schema: "test",
                table: "Vouchers");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "test",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");
        }
    }
}
