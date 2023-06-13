using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchaseStatusMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Status = 'Completed' WHERE Completed = 1 AND Status IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Purchases SET Status = 'Cancelled' WHERE Completed = 0 AND Status IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
