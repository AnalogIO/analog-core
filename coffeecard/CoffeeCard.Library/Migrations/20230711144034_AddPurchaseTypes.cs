using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class AddPurchaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosPurchases",
                schema: "dbo",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    BaristaInitials = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosPurchases", x => x.PurchaseId);
                    table.ForeignKey(
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
            migrationBuilder.DropTable(
                name: "PosPurchases",
                schema: "dbo");
        }
    }
}
