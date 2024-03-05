using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGroupIndexToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_UserGroup",
                schema: "dbo",
                table: "Users",
                column: "UserGroup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserGroup",
                schema: "dbo",
                table: "Users");
        }
    }
}
