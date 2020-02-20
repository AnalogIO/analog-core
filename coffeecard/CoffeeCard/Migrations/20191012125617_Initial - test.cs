using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace coffeecard.Migrations
{
    public partial class Initialtest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "test");

            migrationBuilder.CreateTable(
                "Products",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<int>(),
                    NumberOfTickets = table.Column<int>(),
                    Name = table.Column<string>(),
                    Description = table.Column<string>(nullable: true),
                    ExperienceWorth = table.Column<int>()
                },
                constraints: table => { table.PrimaryKey("PK_Products", x => x.Id); });

            migrationBuilder.CreateTable(
                "Programmes",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    ShortName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    SortPriority = table.Column<int>()
                },
                constraints: table => { table.PrimaryKey("PK_Programmes", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(),
                    DateCreated = table.Column<DateTime>(),
                    DateUpdated = table.Column<DateTime>(),
                    IsVerified = table.Column<bool>(),
                    PrivacyActivated = table.Column<bool>(),
                    Programme_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        "FK_Users_Programmes_Programme_Id",
                        x => x.Programme_Id,
                        principalSchema: "test",
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "LoginAttempts",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Time = table.Column<DateTime>(),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempts", x => x.Id);
                    table.ForeignKey(
                        "FK_LoginAttempts_Users_User_Id",
                        x => x.User_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Purchases",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(),
                    Price = table.Column<int>(),
                    NumberOfTickets = table.Column<int>(),
                    DateCreated = table.Column<DateTime>(),
                    Completed = table.Column<bool>(),
                    OrderId = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    PurchasedBy_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        "FK_Purchases_Users_PurchasedBy_Id",
                        x => x.PurchasedBy_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Statistics",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Preset = table.Column<int>(),
                    SwipeCount = table.Column<int>(),
                    LastSwipe = table.Column<DateTime>(),
                    ExpiryDate = table.Column<DateTime>(),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        "FK_Statistics_Users_User_Id",
                        x => x.User_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Tokens",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    TokenHash = table.Column<string>(nullable: true),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        "FK_Tokens_Users_User_Id",
                        x => x.User_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Vouchers",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(),
                    DateUsed = table.Column<DateTime>(nullable: true),
                    Product_Id = table.Column<int>(nullable: true),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        "FK_Vouchers_Products_Product_Id",
                        x => x.Product_Id,
                        principalSchema: "test",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Vouchers_Users_User_Id",
                        x => x.User_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Tickets",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(),
                    DateUsed = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(),
                    IsUsed = table.Column<bool>(),
                    Owner_Id = table.Column<int>(nullable: true),
                    Purchase_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        "FK_Tickets_Users_Owner_Id",
                        x => x.Owner_Id,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Tickets_Purchases_Purchase_Id",
                        x => x.Purchase_Id,
                        principalSchema: "test",
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_LoginAttempts_User_Id",
                schema: "test",
                table: "LoginAttempts",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Purchases_PurchasedBy_Id",
                schema: "test",
                table: "Purchases",
                column: "PurchasedBy_Id");

            migrationBuilder.CreateIndex(
                "IX_Statistics_User_Id",
                schema: "test",
                table: "Statistics",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Tickets_Owner_Id",
                schema: "test",
                table: "Tickets",
                column: "Owner_Id");

            migrationBuilder.CreateIndex(
                "IX_Tickets_Purchase_Id",
                schema: "test",
                table: "Tickets",
                column: "Purchase_Id");

            migrationBuilder.CreateIndex(
                "IX_Tokens_User_Id",
                schema: "test",
                table: "Tokens",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Users_Programme_Id",
                schema: "test",
                table: "Users",
                column: "Programme_Id");

            migrationBuilder.CreateIndex(
                "IX_Vouchers_Product_Id",
                schema: "test",
                table: "Vouchers",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                "IX_Vouchers_User_Id",
                schema: "test",
                table: "Vouchers",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LoginAttempts",
                "test");

            migrationBuilder.DropTable(
                "Statistics",
                "test");

            migrationBuilder.DropTable(
                "Tickets",
                "test");

            migrationBuilder.DropTable(
                "Tokens",
                "test");

            migrationBuilder.DropTable(
                "Vouchers",
                "test");

            migrationBuilder.DropTable(
                "Purchases",
                "test");

            migrationBuilder.DropTable(
                "Products",
                "test");

            migrationBuilder.DropTable(
                "Users",
                "test");

            migrationBuilder.DropTable(
                "Programmes",
                "test");
        }
    }
}