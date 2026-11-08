using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    /// <inheritdoc />
    public partial class EnforceOneActiveRegistrationLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing environments may already have several usable registration
            // links for one email. Keep the newest and revoke the rest so the
            // unique index can be created.
            migrationBuilder.Sql(
                """
                WITH ranked AS (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [Email] ORDER BY [Id] DESC) AS rn
                    FROM [dbo].[Tokens]
                    WHERE [Type] = 'Registration' AND [Revoked] = 0
                )
                UPDATE t
                SET t.[Revoked] = 1
                FROM [dbo].[Tokens] AS t
                INNER JOIN ranked AS r ON r.[Id] = t.[Id]
                WHERE r.rn > 1;
                """
            );

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Email_ActiveRegistration",
                schema: "dbo",
                table: "Tokens",
                column: "Email",
                unique: true,
                filter: "[Type] = 'Registration' AND [Revoked] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_Email_ActiveRegistration",
                schema: "dbo",
                table: "Tokens");
        }
    }
}
