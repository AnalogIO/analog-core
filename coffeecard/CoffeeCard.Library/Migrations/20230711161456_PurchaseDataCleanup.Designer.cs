﻿// <auto-generated />
using System;
using CoffeeCard.Library.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CoffeeCard.Library.Migrations
{
    [DbContext(typeof(CoffeeCardContext))]
    [Migration("20230711161456_PurchaseDataCleanup")]
    partial class PurchaseDataCleanup
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "6.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CoffeeCard.Models.Entities.LoginAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("LoginAttempts", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.PosPurhase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BastiaInitials")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PurchaseId");

                    b.ToTable("PosPurchases", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
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

                    b.ToTable("Products", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.ProductUserGroup", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "UserGroup");

                    b.ToTable("ProductUserGroups", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Programme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortPriority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Programmes", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumberOfTickets")
                        .HasColumnType("int");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchasedById")
                        .HasColumnType("int")
                        .HasColumnName("PurchasedBy_Id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("PurchasedById");

                    b.HasIndex("TransactionId");

                    b.ToTable("Purchases", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSwipe")
                        .HasColumnType("datetime2");

                    b.Property<int>("Preset")
                        .HasColumnType("int");

                    b.Property<int>("SwipeCount")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Preset", "ExpiryDate");

                    b.ToTable("Statistics", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int")
                        .HasColumnName("Owner_Id");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int")
                        .HasColumnName("Purchase_Id");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PurchaseId");

                    b.ToTable("Tickets", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tokens", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

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

                    b.Property<int>("ProgrammeId")
                        .HasColumnType("int")
                        .HasColumnName("Programme_Id");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.Property<string>("UserState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("ProgrammeId");

                    b.ToTable("Users", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Voucher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("Product_Id");

                    b.Property<int?>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<string>("Requester")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("ProductId");

                    b.HasIndex("PurchaseId");

                    b.HasIndex("UserId");

                    b.ToTable("Vouchers", "dbo");
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

                    b.ToTable("WebhookConfigurations", "dbo");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.LoginAttempt", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("LoginAttempts")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.PosPurhase", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Purchase", "Purchase")
                        .WithMany()
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Purchase");
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
                    b.HasOne("CoffeeCard.Models.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoffeeCard.Models.Entities.User", "PurchasedBy")
                        .WithMany("Purchases")
                        .HasForeignKey("PurchasedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("PurchasedBy");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Statistic", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("Statistics")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Ticket", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "Owner")
                        .WithMany("Tickets")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CoffeeCard.Models.Entities.Purchase", "Purchase")
                        .WithMany("Tickets")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Token", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.User", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Programme", "Programme")
                        .WithMany("Users")
                        .HasForeignKey("ProgrammeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Programme");
                });

            modelBuilder.Entity("CoffeeCard.Models.Entities.Voucher", b =>
                {
                    b.HasOne("CoffeeCard.Models.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoffeeCard.Models.Entities.Purchase", "Purchase")
                        .WithMany()
                        .HasForeignKey("PurchaseId");

                    b.HasOne("CoffeeCard.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Product");

                    b.Navigation("Purchase");

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
