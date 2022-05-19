using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Purchases",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                schema: "dbo",
                table: "Purchases",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Purchases");
        }
    }
}
