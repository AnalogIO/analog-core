using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class WebhookConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebhookConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    SignatureKey = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebhookConfigurations",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases");
        }
    }
}
