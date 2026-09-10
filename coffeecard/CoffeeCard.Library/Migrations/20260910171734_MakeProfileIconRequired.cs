using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class MakeProfileIconRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [dbo].[Users] SET [ProfileIcon] = [Id] % 10 WHERE [ProfileIcon] IS NULL"
            );
            migrationBuilder.Sql(
                "UPDATE [dbo].[Users] SET [ProfileBackgroundColor] = [Id] % 10 WHERE [ProfileBackgroundColor] IS NULL"
            );

            migrationBuilder.AlterColumn<int>(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProfileBackgroundColor",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProfileIcon",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileBackgroundColor",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
