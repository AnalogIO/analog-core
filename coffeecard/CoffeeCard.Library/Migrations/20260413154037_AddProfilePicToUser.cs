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
                name: "ProfileBackgroundColor",
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

            // Assign values to existing users using the same rules as the UserIcon widget in coffeecard_app:
            // icon = id % 9, background color = id % 10
            migrationBuilder.Sql(
                "UPDATE [dbo].[Users] SET [ProfileIcon] = [Id] % 9");
            migrationBuilder.Sql(
                "UPDATE [dbo].[Users] SET [ProfileBackgroundColor] = [Id] % 10");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileBackgroundColor",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileBackgroundColor",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users");
        }
    }
}
