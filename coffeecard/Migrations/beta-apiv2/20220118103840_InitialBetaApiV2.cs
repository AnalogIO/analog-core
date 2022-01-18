using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.Library.Migrations
{
    public partial class InitialBetaApiV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "beta-apiv2");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<int>(nullable: false),
                    NumberOfTickets = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExperienceWorth = table.Column<int>(nullable: false),
                    Visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    SortPriority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductUserGroups",
                schema: "beta-apiv2",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    UserGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUserGroups", x => new { x.ProductId, x.UserGroup });
                    table.ForeignKey(
                        name: "FK_ProductUserGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "beta-apiv2",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false),
                    Experience = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    PrivacyActivated = table.Column<bool>(nullable: false),
                    UserGroup = table.Column<int>(nullable: false),
                    Programme_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Programmes_Programme_Id",
                        column: x => x.Programme_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempts",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(nullable: false),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempts_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    NumberOfTickets = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    OrderId = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    PurchasedBy_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Users_PurchasedBy_Id",
                        column: x => x.PurchasedBy_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preset = table.Column<int>(nullable: false),
                    SwipeCount = table.Column<int>(nullable: false),
                    LastSwipe = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(nullable: true),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUsed = table.Column<DateTime>(nullable: true),
                    Product_Id = table.Column<int>(nullable: true),
                    User_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_Products_Product_Id",
                        column: x => x.Product_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "beta-apiv2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUsed = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    Owner_Id = table.Column<int>(nullable: true),
                    Purchase_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_Owner_Id",
                        column: x => x.Owner_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Purchases_Purchase_Id",
                        column: x => x.Purchase_Id,
                        principalSchema: "beta-apiv2",
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_User_Id",
                schema: "beta-apiv2",
                table: "LoginAttempts",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchasedBy_Id",
                schema: "beta-apiv2",
                table: "Purchases",
                column: "PurchasedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_User_Id",
                schema: "beta-apiv2",
                table: "Statistics",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Owner_Id",
                schema: "beta-apiv2",
                table: "Tickets",
                column: "Owner_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Purchase_Id",
                schema: "beta-apiv2",
                table: "Tickets",
                column: "Purchase_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_User_Id",
                schema: "beta-apiv2",
                table: "Tokens",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Programme_Id",
                schema: "beta-apiv2",
                table: "Users",
                column: "Programme_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Product_Id",
                schema: "beta-apiv2",
                table: "Vouchers",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_User_Id",
                schema: "beta-apiv2",
                table: "Vouchers",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAttempts",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "ProductUserGroups",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Statistics",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Vouchers",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Purchases",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "beta-apiv2");

            migrationBuilder.DropTable(
                name: "Programmes",
                schema: "beta-apiv2");
        }
    }
}
