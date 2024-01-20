using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class RenameMenuItemColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_MenuItems_MenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_Products_ProductsId",
                schema: "dbo",
                table: "MenuItemProduct");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "EligibleMenuItemsId");

            migrationBuilder.RenameColumn(
                name: "MenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "AssociatedProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProduct_ProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "IX_MenuItemProduct_EligibleMenuItemsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "MenuItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Name",
                schema: "dbo",
                table: "MenuItems",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_MenuItems_EligibleMenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct",
                column: "EligibleMenuItemsId",
                principalSchema: "dbo",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_Products_AssociatedProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                column: "AssociatedProductsId",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_MenuItems_EligibleMenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_Products_AssociatedProductsId",
                schema: "dbo",
                table: "MenuItemProduct");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_Name",
                schema: "dbo",
                table: "MenuItems");

            migrationBuilder.RenameColumn(
                name: "EligibleMenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "ProductsId");

            migrationBuilder.RenameColumn(
                name: "AssociatedProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "MenuItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProduct_EligibleMenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct",
                newName: "IX_MenuItemProduct_ProductsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_MenuItems_MenuItemsId",
                schema: "dbo",
                table: "MenuItemProduct",
                column: "MenuItemsId",
                principalSchema: "dbo",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_Products_ProductsId",
                schema: "dbo",
                table: "MenuItemProduct",
                column: "ProductsId",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
