using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.WebApi.Migrations.Prod
{
    public partial class ProductUserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "Vouchers",
                newName: "Vouchers",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Tokens",
                newName: "Tokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Tickets",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Statistics",
                newName: "Statistics",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchases",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Programmes",
                newName: "Programmes",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "LoginAttempts",
                newName: "LoginAttempts",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserGroup",
                schema: "dbo",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "dbo",
                table: "Products",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "ProductUserGroups",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    UserGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUserGroups", x => new { x.ProductId, x.UserGroup });
                    table.ForeignKey(
                        name: "FK_ProductUserGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUserGroups",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "UserGroup",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "dbo",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Vouchers",
                schema: "dbo",
                newName: "Vouchers");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "dbo",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Tokens",
                schema: "dbo",
                newName: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "dbo",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "Statistics",
                schema: "dbo",
                newName: "Statistics");

            migrationBuilder.RenameTable(
                name: "Purchases",
                schema: "dbo",
                newName: "Purchases");

            migrationBuilder.RenameTable(
                name: "Programmes",
                schema: "dbo",
                newName: "Programmes");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "dbo",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "LoginAttempts",
                schema: "dbo",
                newName: "LoginAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
