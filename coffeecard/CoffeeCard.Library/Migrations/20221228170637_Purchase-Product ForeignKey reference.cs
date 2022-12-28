using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseProductForeignKeyreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId",
                schema: "dbo",
                table: "Purchases",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Products_ProductId",
                schema: "dbo",
                table: "Purchases",
                column: "ProductId",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Products_ProductId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ProductId",
                schema: "dbo",
                table: "Purchases");
        }
    }
}
