using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CoffeeCard.Library.Migrations
{
    public partial class initialpostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    NumberOfTickets = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExperienceWorth = table.Column<int>(type: "integer", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    SortPriority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductUserGroups",
                schema: "dbo",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    UserGroup = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUserGroups", x => new { x.ProductId, x.UserGroup });
                    table.ForeignKey(
                        name: "FK_ProductUserGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PrivacyActivated = table.Column<bool>(type: "boolean", nullable: false),
                    UserGroup = table.Column<int>(type: "integer", nullable: false),
                    Programme_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Programmes_Programme_Id",
                        column: x => x.Programme_Id,
                        principalSchema: "dbo",
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempts_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    NumberOfTickets = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: true),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    PurchasedBy_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Users_PurchasedBy_Id",
                        column: x => x.PurchasedBy_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Preset = table.Column<int>(type: "integer", nullable: false),
                    SwipeCount = table.Column<int>(type: "integer", nullable: false),
                    LastSwipe = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    User_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenHash = table.Column<string>(type: "text", nullable: true),
                    User_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Product_Id = table.Column<int>(type: "integer", nullable: true),
                    User_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_Products_Product_Id",
                        column: x => x.Product_Id,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Owner_Id = table.Column<int>(type: "integer", nullable: true),
                    Purchase_Id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Purchases_Purchase_Id",
                        column: x => x.Purchase_Id,
                        principalSchema: "dbo",
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_Owner_Id",
                        column: x => x.Owner_Id,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Products",
                columns: new[] { "Id", "Description", "ExperienceWorth", "Name", "NumberOfTickets", "Price", "Visible" },
                values: new object[,]
                {
                    { 1, "Used for filter coffee brewed with fresh ground coffee", 10, "Filter Coffee", 10, 80, true },
                    { 2, "Used for specialities like espresso, cappuccino, caffe latte, cortado, americano and chai latte", 150, "Espresso Based", 10, 150, true }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Programmes",
                columns: new[] { "Id", "FullName", "ShortName", "SortPriority" },
                values: new object[,]
                {
                    { 1, "BSc Software Development", "SWU", 0 },
                    { 2, "BSc Global Business Informatics", "GBI", 0 },
                    { 3, "BSc Digital Design and Interactive Technologies", "BDDIT", 0 },
                    { 4, "MSc Digital Design and Interactive Technologies", "KDDIT", 0 },
                    { 5, "MSc Digital Innovation and Management", "DIM", 0 },
                    { 6, "MSc E-Business", "E-BUSS", 0 },
                    { 7, "MSc Games - Design and Theory", "GAMES/DT", 0 },
                    { 8, "MSc Games - Technology", "GAMES/Tech", 0 },
                    { 9, "MSc Computer Science", "CS", 0 },
                    { 10, "MSc Software Development (Design)", "SDT", 0 },
                    { 11, "Employee", "Employee", 0 },
                    { 12, "Other", "Other", 0 },
                    { 13, "BSc Data Science", "DS", 0 }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "ProductUserGroups",
                columns: new[] { "ProductId", "UserGroup" },
                values: new object[,]
                {
                    { 1, 0 },
                    { 2, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_User_Id",
                schema: "dbo",
                table: "LoginAttempts",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchasedBy_Id",
                schema: "dbo",
                table: "Purchases",
                column: "PurchasedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_User_Id",
                schema: "dbo",
                table: "Statistics",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Owner_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Owner_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Purchase_Id",
                schema: "dbo",
                table: "Tickets",
                column: "Purchase_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_User_Id",
                schema: "dbo",
                table: "Tokens",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Programme_Id",
                schema: "dbo",
                table: "Users",
                column: "Programme_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Product_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_User_Id",
                schema: "dbo",
                table: "Vouchers",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAttempts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductUserGroups",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Statistics",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Vouchers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Purchases",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Programmes",
                schema: "dbo");
        }
    }
}
