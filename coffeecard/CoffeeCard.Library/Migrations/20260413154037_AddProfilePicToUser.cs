using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePicToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PictureBackgroundColor",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureBackgroundColor",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users");
        }
    }
}
