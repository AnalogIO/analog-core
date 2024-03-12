using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class PurchasesDropCompletedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "Completed",
                schema: "dbo",
                table: "Purchases");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AddColumn<bool>(
                name: "Completed",
                schema: "dbo",
                table: "Purchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.Sql("UPDATE dbo.Purchases SET Completed = 0 WHERE Status != 'Completed'");
            _ = migrationBuilder.Sql("UPDATE dbo.Purchases SET Completed = 1 WHERE Status = 'Completed'");
        }
    }
}
