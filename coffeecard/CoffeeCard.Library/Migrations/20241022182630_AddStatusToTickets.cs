using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Update the Status column according to the IsUsed column
            migrationBuilder.Sql("UPDATE dbo.Tickets SET Status = 1 WHERE IsUsed = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Update the IsUsed column according to the Status column
            migrationBuilder.Sql("UPDATE dbo.Tickets SET IsUsed = 1 WHERE Status = 1 OR Status = 2");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Tickets");
        }
    }
}
