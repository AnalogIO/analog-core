using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class AddedMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemProduct",
                schema: "dbo",
                columns: table => new
                {
                    MenuItemsId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemProduct", x => new { x.MenuItemsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_MenuItemProduct_MenuItems_MenuItemsId",
                        column: x => x.MenuItemsId,
                        principalSchema: "dbo",
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets",
                column: "UsedOnMenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemProduct_ProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_MenuItems_UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets",
                column: "UsedOnMenuItemId",
                principalSchema: "dbo",
                principalTable: "MenuItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_MenuItems_UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "MenuItemProduct",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "UsedOnMenuItemId",
                schema: "dbo",
                table: "Tickets");
        }
    }
}
