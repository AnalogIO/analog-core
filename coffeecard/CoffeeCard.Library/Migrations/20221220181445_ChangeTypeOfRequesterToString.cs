using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class ChangeTypeOfRequesterToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Users_Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.AddColumn<string>(
                name: "Requester",
                schema: "dbo",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Requester",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.AddColumn<int>(
                name: "Requester_Id",
                schema: "dbo",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Requester_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "Requester_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Users_Requester_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "Requester_Id",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
