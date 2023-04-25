using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class UpdateTransactionIdChangecolumnnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Purchases_PurchaseId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_OwnerId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "PurchaseId",
                schema: "dbo",
                table: "Tickets",
                newName: "Purchase_Id");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "dbo",
                table: "Tickets",
                newName: "Owner_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_PurchaseId",
                schema: "dbo",
                table: "Tickets",
                newName: "IX_Tickets_Purchase_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OwnerId",
                schema: "dbo",
                table: "Tickets",
                newName: "IX_Tickets_Owner_Id");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Purchases_Purchase_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Purchase_Id",
                principalSchema: "dbo",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_Owner_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Owner_Id",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Purchases_Purchase_Id",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_Owner_Id",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Purchase_Id",
                schema: "dbo",
                table: "Tickets",
                newName: "PurchaseId");

            migrationBuilder.RenameColumn(
                name: "Owner_Id",
                schema: "dbo",
                table: "Tickets",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_Purchase_Id",
                schema: "dbo",
                table: "Tickets",
                newName: "IX_Tickets_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_Owner_Id",
                schema: "dbo",
                table: "Tickets",
                newName: "IX_Tickets_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Purchases_PurchaseId",
                schema: "dbo",
                table: "Tickets",
                column: "PurchaseId",
                principalSchema: "dbo",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_OwnerId",
                schema: "dbo",
                table: "Tickets",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
