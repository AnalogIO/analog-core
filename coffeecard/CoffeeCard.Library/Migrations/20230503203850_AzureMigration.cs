using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    public partial class AzureMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.EnsureSchema(
                name: "dbo");

            _ = migrationBuilder.CreateTable(
                name: "Products",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<int>(type: "int", nullable: false),
                    NumberOfTickets = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceWorth = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Products", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "Programmes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortPriority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Programmes", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "WebhookConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_WebhookConfigurations", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "ProductUserGroups",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserGroup = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ProductUserGroups", x => new { x.ProductId, x.UserGroup });
                    _ = table.ForeignKey(
                        name: "FK_ProductUserGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyActivated = table.Column<bool>(type: "bit", nullable: false),
                    UserGroup = table.Column<int>(type: "int", nullable: false),
                    UserState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Programme_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Users", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Users_Programmes_Programme_Id",
                        column: x => x.Programme_Id,
                        principalSchema: "dbo",
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "LoginAttempts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_LoginAttempts", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_LoginAttempts_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    NumberOfTickets = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasedBy_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Purchases", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Purchases_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_Purchases_Users_PurchasedBy_Id",
                        column: x => x.PurchasedBy_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "Statistics",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preset = table.Column<int>(type: "int", nullable: false),
                    SwipeCount = table.Column<int>(type: "int", nullable: false),
                    LastSwipe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Statistics", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Statistics_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Tokens", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Tokens_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateTable(
                name: "Vouchers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requester = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Product_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Vouchers", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Vouchers_Products_Product_Id",
                        column: x => x.Product_Id,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_Vouchers_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Owner_Id = table.Column<int>(type: "int", nullable: false),
                    Purchase_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Tickets", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Tickets_Purchases_Purchase_Id",
                        column: x => x.Purchase_Id,
                        principalSchema: "dbo",
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_Tickets_Users_Owner_Id",
                        column: x => x.Owner_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_User_Id",
                schema: "dbo",
                table: "LoginAttempts",
                column: "User_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Purchases_OrderId",
                schema: "dbo",
                table: "Purchases",
                column: "OrderId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId",
                schema: "dbo",
                table: "Purchases",
                column: "ProductId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases",
                column: "PurchasedBy_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                schema: "dbo",
                table: "Purchases",
                column: "TransactionId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Statistics_User_Id",
                schema: "dbo",
                table: "Statistics",
                column: "User_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Tickets_Owner_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Owner_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Tickets_Purchase_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Purchase_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Tokens_User_Id",
                schema: "dbo",
                table: "Tokens",
                column: "User_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Users_Programme_Id",
                schema: "dbo",
                table: "Users",
                column: "Programme_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                schema: "dbo",
                table: "Vouchers",
                column: "Code",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Product_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "Product_Id");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Vouchers_User_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "LoginAttempts",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "ProductUserGroups",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Statistics",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Tickets",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Tokens",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Vouchers",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "WebhookConfigurations",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Purchases",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Products",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            _ = migrationBuilder.DropTable(
                name: "Programmes",
                schema: "dbo");
        }
    }
}
