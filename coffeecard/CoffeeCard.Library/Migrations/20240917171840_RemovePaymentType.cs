using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases",
                column: "PurchasedBy_Id",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases",
                column: "PurchasedBy_Id",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
