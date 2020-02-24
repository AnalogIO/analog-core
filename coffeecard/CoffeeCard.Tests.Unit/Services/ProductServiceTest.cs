using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Models;
using CoffeeCard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class ProductServiceTest
    {
        [Fact(DisplayName = "GetPublicProducts return all non-barista products")]
        public async Task GetPublicProducts_Return_All_NonBarista_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeecardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_Return_All_NonBarista_Products));

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["databaseSchema"]).Returns("test");

            using (var context = new CoffeecardContext(builder.Options, configuration.Object))
            {
                await context.AddAsync(new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    BaristasOnly = false,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    BaristasOnly = false,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 3,
                    Name = "Barista Coffee",
                    Description = "Barista Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    BaristasOnly = true,
                    Visible = true
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            BaristasOnly = false,
                            Visible = true
                        },
                        new Product()
                        {
                            Id = 2,
                            Name = "Espresso",
                            Description = "Espresso Clip card",
                            NumberOfTickets = 10,
                            Price = 20,
                            ExperienceWorth = 20,
                            BaristasOnly = false,
                            Visible = true
                        }
                    };

                    var result = await productService.GetPublicProducts();

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetPublicProducts does not return non-visible products")]
        public async Task GetPublicProducts_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeecardContext>()
                .UseInMemoryDatabase(nameof(GetPublicProducts_DoesNot_Return_NonVisible_Products));

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["databaseSchema"]).Returns("test");

            using (var context = new CoffeecardContext(builder.Options, configuration.Object))
            {
                await context.AddAsync(new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    BaristasOnly = false,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 2,
                    Name = "Hidden Coffee",
                    Description = "Hidden Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    BaristasOnly = false,
                    Visible = false
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            BaristasOnly = false,
                            Visible = true
                        },
                    };

                    var result = await productService.GetPublicProducts();

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetProductsForUserAsync return all products")]
        public async Task GetProductsForUserAsync_Return_All_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeecardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_Return_All_Products));

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["databaseSchema"]).Returns("test");

            using (var context = new CoffeecardContext(builder.Options, configuration.Object))
            {
                await context.AddAsync(new Product()
                {
                    Id = 1,
                    Name = "Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    BaristasOnly = false,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Espresso Clip card",
                    NumberOfTickets = 10,
                    Price = 20,
                    ExperienceWorth = 20,
                    BaristasOnly = false,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 3,
                    Name = "Barista Coffee",
                    Description = "Barista Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    BaristasOnly = true,
                    Visible = true
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            BaristasOnly = false,
                            Visible = true
                        },
                        new Product()
                        {
                            Id = 2,
                            Name = "Espresso",
                            Description = "Espresso Clip card",
                            NumberOfTickets = 10,
                            Price = 20,
                            ExperienceWorth = 20,
                            BaristasOnly = false,
                            Visible = true
                        },
                        new Product()
                        {
                            Id = 3,
                            Name = "Barista Coffee",
                            Description = "Barista Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 30,
                            ExperienceWorth = 10,
                            BaristasOnly = true,
                            Visible = true
                        }
                    };

                    var user = new User()
                    {
                        IsBarista = true
                    };

                    var result = await productService.GetProductsForUserAsync(user);

                    Assert.Equal(expected, result);
                }
            }
        }

        [Fact(DisplayName = "GetProductsForUserAsync does not return non-visible products")]
        public async Task GetProductsForUserAsync_DoesNot_Return_NonVisible_Products()
        {
            var builder = new DbContextOptionsBuilder<CoffeecardContext>()
                .UseInMemoryDatabase(nameof(GetProductsForUserAsync_DoesNot_Return_NonVisible_Products));

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["databaseSchema"]).Returns("test");

            using (var context = new CoffeecardContext(builder.Options, configuration.Object))
            {
                await context.AddAsync(new Product()
                {
                    Id = 1,
                    Name = "Barista Coffee",
                    Description = "Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 10,
                    ExperienceWorth = 10,
                    BaristasOnly = true,
                    Visible = true
                });
                await context.AddAsync(new Product()
                {
                    Id = 2,
                    Name = "Hidden Coffee",
                    Description = "Hidden Coffee Clip card",
                    NumberOfTickets = 10,
                    Price = 30,
                    ExperienceWorth = 10,
                    BaristasOnly = false,
                    Visible = false
                });
                await context.SaveChangesAsync();

                using (var productService = new ProductService(context))
                {
                    var expected = new List<Product>
                    {
                        new Product()
                        {
                            Id = 1,
                            Name = "Barista Coffee",
                            Description = "Coffee Clip card",
                            NumberOfTickets = 10,
                            Price = 10,
                            ExperienceWorth = 10,
                            BaristasOnly = true,
                            Visible = true
                        },
                    };

                    var user = new User()
                    {
                        IsBarista = true
                    };

                    var result = await productService.GetProductsForUserAsync(user);

                    Assert.Equal(expected, result);
                }
            }
        }
    }
}
