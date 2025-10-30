using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasedByToTicketAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE dbo.Tickets 
                SET PurchasedBy_Id = Owner_Id 
                WHERE PurchasedBy_Id IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets",
                column: "PurchasedBy_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets",
                column: "PurchasedBy_Id",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PurchasedBy_Id",
                schema: "dbo",
                table: "Tickets");
        }
    }
}
