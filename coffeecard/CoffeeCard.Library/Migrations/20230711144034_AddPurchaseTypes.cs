using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class AddPurchaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<int>(
                name: "PurchaseId",
                schema: "dbo",
                table: "Vouchers",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            _ = migrationBuilder.CreateTable(
                name: "PosPurchases",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    BaristaInitials = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_PosPurchases", x => x.PurchaseId);
                    _ = table.ForeignKey(
                        name: "FK_PosPurchases_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalSchema: "dbo",
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "PosPurchases",
                schema: "dbo");
        }
    }
}
