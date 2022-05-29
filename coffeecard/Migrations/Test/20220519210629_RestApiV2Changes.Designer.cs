﻿// <auto-generated />
using System;
using CoffeeCard.Library.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoffeeCard.WebApi.Migrations
{
    [DbContext(typeof(CoffeeCardContext))]
    [Migration("20220519210629_RestApiV2Changes")]
    partial class RestApiV2Changes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("test")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoffeeCard.Models.Entities.LoginAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("LoginAttempts");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExperienceWorth")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfTickets")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<bool>("Visible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Used for filter coffee brewed with fresh ground coffee",
                            ExperienceWorth = 10,
                            Name = "Filter Coffee",
                            NumberOfTickets = 10,
                            Price = 80,
                            Visible = true
                        },
                        new
                        {
                            Id = 2,
                            Description = "Used for specialities like espresso, cappuccino, caffe latte, cortado, americano and chai latte",
                            ExperienceWorth = 150,
                            Name = "Espresso Based",
                            NumberOfTickets = 10,
                            Price = 150,
                            Visible = true
                        });
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.ProductUserGroup", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "UserGroup");

                    b.ToTable("ProductUserGroups");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            UserGroup = 0
                        },
                        new
                        {
                            ProductId = 2,
                            UserGroup = 0
                        });
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Programme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortPriority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Programmes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FullName = "BSc Software Development",
                            ShortName = "SWU",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 2,
                            FullName = "BSc Global Business Informatics",
                            ShortName = "GBI",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 3,
                            FullName = "BSc Digital Design and Interactive Technologies",
                            ShortName = "BDDIT",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 4,
                            FullName = "MSc Digital Design and Interactive Technologies",
                            ShortName = "KDDIT",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 5,
                            FullName = "MSc Digital Innovation and Management",
                            ShortName = "DIM",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 6,
                            FullName = "MSc E-Business",
                            ShortName = "E-BUSS",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 7,
                            FullName = "MSc Games - Design and Theory",
                            ShortName = "GAMES/DT",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 8,
                            FullName = "MSc Games - Technology",
                            ShortName = "GAMES/Tech",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 9,
                            FullName = "MSc Computer Science",
                            ShortName = "CS",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 10,
                            FullName = "MSc Software Development (Design)",
                            ShortName = "SDT",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 11,
                            FullName = "Employee",
                            ShortName = "Employee",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 12,
                            FullName = "Other",
                            ShortName = "Other",
                            SortPriority = 0
                        },
                        new
                        {
                            Id = 13,
                            FullName = "BSc Data Science",
                            ShortName = "DS",
                            SortPriority = 0
                        });
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumberOfTickets")
                        .HasColumnType("int");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PurchasedBy_Id")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique()
                        .HasFilter("[OrderId] IS NOT NULL");

                    b.HasIndex("PurchasedBy_Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSwipe")
                        .HasColumnType("datetime2");

                    b.Property<int>("Preset")
                        .HasColumnType("int");

                    b.Property<int>("SwipeCount")
                        .HasColumnType("int");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<int?>("Owner_Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("Purchase_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Owner_Id");

                    b.HasIndex("Purchase_Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TokenHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PrivacyActivated")
                        .HasColumnType("bit");

                    b.Property<int?>("Programme_Id")
                        .HasColumnType("int");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.Property<string>("UserState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Programme_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Voucher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Product_Id")
                        .HasColumnType("int");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Product_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Vouchers");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.WebhookConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("SignatureKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WebhookConfigurations");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.LoginAttempt", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("LoginAttempts")
                        .HasForeignKey("User_Id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.ProductUserGroup", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Product", "Product")
                        .WithMany("ProductUserGroup")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Purchase", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "PurchasedBy")
                        .WithMany("Purchases")
                        .HasForeignKey("PurchasedBy_Id");

                    b.Navigation("PurchasedBy");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Statistic", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("Statistics")
                        .HasForeignKey("User_Id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Ticket", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "Owner")
                        .WithMany("Tickets")
                        .HasForeignKey("Owner_Id");

                    b.HasOne("CoffeeCard.Models.Entities.Purchase", "Purchase")
                        .WithMany("Tickets")
                        .HasForeignKey("Purchase_Id");

                    b.Navigation("Owner");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Token", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("User_Id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.User", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Programme", "Programme")
                        .WithMany("Users")
                        .HasForeignKey("Programme_Id");

                    b.Navigation("Programme");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Voucher", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("Product_Id");

                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("User_Id");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Product", b =>
                {
                    b.Navigation("ProductUserGroup");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Programme", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Purchase", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.User", b =>
                {
                    b.Navigation("LoginAttempts");

                    b.Navigation("Purchases");

                    b.Navigation("Statistics");

                    b.Navigation("Tickets");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}