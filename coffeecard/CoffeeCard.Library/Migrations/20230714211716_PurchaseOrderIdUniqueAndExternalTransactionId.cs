using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseOrderIdUniqueAndExternalTransactionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make transactionId null, where not MobilePayV1, or MobilepayV2, and generate unique Guid orderIds
            migrationBuilder.Sql("update Purchases set OrderId = NEWID(), TransactionId=null where Type != 'MobilePayV1' and Type != 'MobilePayV2' or OrderId = 'OLD PURCHASES'");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                schema: "dbo",
                table: "Purchases",
                newName: "ExternalTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_TransactionId",
                schema: "dbo",
                table: "Purchases",
                newName: "IX_Purchases_ExternalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "ExternalTransactionId",
                schema: "dbo",
                table: "Purchases",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_ExternalTransactionId",
                schema: "dbo",
                table: "Purchases",
                newName: "IX_Purchases_TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId");
        }
    }
}
