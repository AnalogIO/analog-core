using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class StatisticIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Statistics_Preset_ExpiryDate",
                schema: "dbo",
                table: "Statistics",
                columns: new[] { "Preset", "ExpiryDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Statistics_Preset_ExpiryDate",
                schema: "dbo",
                table: "Statistics");
        }
    }
}
