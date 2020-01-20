using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace coffeecard.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Products",
                table => new
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
                table => new
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
                table => new
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
                        "Programmes",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "LoginAttempts",
                table => new
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
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Purchases",
                table => new
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
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Statistics",
                table => new
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
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Tokens",
                table => new
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
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Vouchers",
                table => new
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
                        "Products",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Vouchers_Users_User_Id",
                        x => x.User_Id,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Tickets",
                table => new
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
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Tickets_Purchases_Purchase_Id",
                        x => x.Purchase_Id,
                        "Purchases",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_LoginAttempts_User_Id",
                "LoginAttempts",
                "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Purchases_PurchasedBy_Id",
                "Purchases",
                "PurchasedBy_Id");

            migrationBuilder.CreateIndex(
                "IX_Statistics_User_Id",
                "Statistics",
                "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Tickets_Owner_Id",
                "Tickets",
                "Owner_Id");

            migrationBuilder.CreateIndex(
                "IX_Tickets_Purchase_Id",
                "Tickets",
                "Purchase_Id");

            migrationBuilder.CreateIndex(
                "IX_Tokens_User_Id",
                "Tokens",
                "User_Id");

            migrationBuilder.CreateIndex(
                "IX_Users_Programme_Id",
                "Users",
                "Programme_Id");

            migrationBuilder.CreateIndex(
                "IX_Vouchers_Product_Id",
                "Vouchers",
                "Product_Id");

            migrationBuilder.CreateIndex(
                "IX_Vouchers_User_Id",
                "Vouchers",
                "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LoginAttempts");

            migrationBuilder.DropTable(
                "Statistics");

            migrationBuilder.DropTable(
                "Tickets");

            migrationBuilder.DropTable(
                "Tokens");

            migrationBuilder.DropTable(
                "Vouchers");

            migrationBuilder.DropTable(
                "Purchases");

            migrationBuilder.DropTable(
                "Products");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Programmes");
        }
    }
}