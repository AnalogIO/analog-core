using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class MenuItemProducts : Migration
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
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemProducts",
                schema: "dbo",
                columns: table => new
                {
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemProducts", x => new { x.MenuItemId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_MenuItemProducts_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalSchema: "dbo",
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemProducts_Products_ProductId",
                        column: x => x.ProductId,
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
                name: "IX_MenuItemProducts_ProductId",
                schema: "dbo",
                table: "MenuItemProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Name",
                schema: "dbo",
                table: "MenuItems",
                column: "Name",
                unique: true);

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
                name: "MenuItemProducts",
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
