using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class AddDescriptionAndRequesterToVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "Vouchers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Requester_Id",
                schema: "dbo",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                schema: "dbo",
                table: "Vouchers",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Users_Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Code",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Requester_Id",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
