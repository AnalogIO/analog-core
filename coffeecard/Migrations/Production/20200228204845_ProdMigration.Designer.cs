﻿// <auto-generated />

using System;
using CoffeeCard.Library.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoffeeCard.WebApi.Migrations
{
    [DbContext(typeof(CoffeeCardContext))]
    [Migration("20200228204845_ProdMigration")]
    partial class ProdMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoffeeCard.Models.LoginAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Time");

                    b.Property<int?>("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("LoginAttempts");
                });

            modelBuilder.Entity("CoffeeCard.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int>("ExperienceWorth");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfTickets");

                    b.Property<int>("Price");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CoffeeCard.Models.Programme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FullName");

                    b.Property<string>("ShortName");

                    b.Property<int>("SortPriority");

                    b.HasKey("Id");

                    b.ToTable("Programmes");
                });

            modelBuilder.Entity("CoffeeCard.Models.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Completed");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("NumberOfTickets");

                    b.Property<string>("OrderId");

                    b.Property<int>("Price");

                    b.Property<int>("ProductId");

                    b.Property<string>("ProductName");

                    b.Property<int?>("PurchasedBy_Id");

                    b.Property<string>("TransactionId");

                    b.HasKey("Id");

                    b.HasIndex("PurchasedBy_Id");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("CoffeeCard.Models.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ExpiryDate");

                    b.Property<DateTime>("LastSwipe");

                    b.Property<int>("Preset");

                    b.Property<int>("SwipeCount");

                    b.Property<int?>("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("CoffeeCard.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUsed");

                    b.Property<bool>("IsUsed");

                    b.Property<int?>("Owner_Id");

                    b.Property<int>("ProductId");

                    b.Property<int?>("Purchase_Id");

                    b.HasKey("Id");

                    b.HasIndex("Owner_Id");

                    b.HasIndex("Purchase_Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("CoffeeCard.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TokenHash");

                    b.Property<int?>("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("CoffeeCard.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Email");

                    b.Property<int>("Experience");

                    b.Property<bool>("IsVerified");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<bool>("PrivacyActivated");

                    b.Property<int?>("Programme_Id");

                    b.Property<string>("Salt");

                    b.HasKey("Id");

                    b.HasIndex("Programme_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CoffeeCard.Models.Voucher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUsed");

                    b.Property<int?>("Product_Id");

                    b.Property<int?>("User_Id");

                    b.HasKey("Id");

                    b.HasIndex("Product_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Vouchers");
                });

            modelBuilder.Entity("CoffeeCard.Models.LoginAttempt", b =>
                {
                    b.HasOne("CoffeeCard.Models.User", "User")
                        .WithMany("LoginAttempts")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.Purchase", b =>
                {
                    b.HasOne("CoffeeCard.Models.User", "PurchasedBy")
                        .WithMany("Purchases")
                        .HasForeignKey("PurchasedBy_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.Statistic", b =>
                {
                    b.HasOne("CoffeeCard.Models.User", "User")
                        .WithMany("Statistics")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.Ticket", b =>
                {
                    b.HasOne("CoffeeCard.Models.User", "Owner")
                        .WithMany("Tickets")
                        .HasForeignKey("Owner_Id");

                    b.HasOne("CoffeeCard.Models.Purchase", "Purchase")
                        .WithMany("Tickets")
                        .HasForeignKey("Purchase_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.Token", b =>
                {
                    b.HasOne("CoffeeCard.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.User", b =>
                {
                    b.HasOne("CoffeeCard.Models.Programme", "Programme")
                        .WithMany("Users")
                        .HasForeignKey("Programme_Id");
                });

            modelBuilder.Entity("CoffeeCard.Models.Voucher", b =>
                {
                    b.HasOne("CoffeeCard.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("Product_Id");

                    b.HasOne("CoffeeCard.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("User_Id");
                });
#pragma warning restore 612, 618
        }
    }
}
