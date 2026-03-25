using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedVoucherIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_PurchaseId",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PurchaseId",
                schema: "dbo",
                table: "Vouchers",
                column: "PurchaseId",
                unique: true,
                filter: "[PurchaseId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vouchers_PurchaseId",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PurchaseId",
                schema: "dbo",
                table: "Vouchers",
                column: "PurchaseId");
        }
    }
}
