using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.WebApi.Migrations
{
    public partial class RestApiV2Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserState",
                schema: "test",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                schema: "test",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                schema: "test",
                table: "Purchases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "test",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WebhookConfigurations",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookConfigurations", x => x.Id);
                });


            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                schema: "test",
                table: "Purchases",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebhookConfigurations",
                schema: "test");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "test",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                schema: "test",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UserState",
                schema: "test",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "test",
                table: "Purchases");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                schema: "test",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                schema: "test",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
